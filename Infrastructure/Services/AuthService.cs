using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Domain.Constants;
using Domain.DTOs.Auth;
using Domain.DTOs.Email;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService;
    private readonly DataContext _context;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<IdentityUser> userManager,
        IConfiguration config,
        IEmailService emailService,
        DataContext context,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _config = config;
        _emailService = emailService;
        _context = context;
        _logger = logger;
    }
    public async Task<Response<TokenDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.UserName);
        if (user == null)
        {
            return new Response<TokenDto>(HttpStatusCode.BadRequest, "Username or password is incorrect");
        }

        var checkPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!checkPassword)
        {
            return new Response<TokenDto>(HttpStatusCode.BadRequest, "Username or password is incorrect");
        }

        var token = await GenerateJwt(user);
        return new Response<TokenDto>(new TokenDto { Token = token });
    }

    public async Task<Response<string>> Register(RegisterDto registerDto)
    {
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
            return new Response<string>(HttpStatusCode.Conflict, "Email already in use");
        
        var user = new IdentityUser
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email,
        };
        
        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return new Response<string>(HttpStatusCode.InternalServerError, $"Failed to create user: {errors}");
        }


        await _userManager.AddToRoleAsync(user, Roles.User);
        return new Response<string>("User created");
    }

    public async Task<Response<string>> RequestResetPassword(RequestResetPassword requestResetPassword)
    {
        var user = await _userManager.FindByEmailAsync(requestResetPassword.Email);
        if (user == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var emailDto = new EmailDto()
        {
            To = requestResetPassword.Email,
            Subject = "Reset Password",
            Body = $"Your token is {token}",
        };

        var result = await _emailService.SendEmailAsync(emailDto);

        return !result
            ? new Response<string>(HttpStatusCode.InternalServerError, "Failed to send email")
            : new Response<string>("Token sent successfully to email");
    }

    public async Task<Response<string>> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        var resetResult =
            await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

        return resetResult.Succeeded
            ? new Response<string>("Password reset successfully")
            : new Response<string>(HttpStatusCode.InternalServerError, "Failed to reset password");
    }


    private async Task<string> GenerateJwt(IdentityUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? "")
        };

        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<string>> ChangeRole(ChangeRoleDto changeRoleDto)
    {
        _logger.LogInformation("Searching user for changing");
        var user = await _userManager.FindByIdAsync(changeRoleDto.UserId);
        if (user == null)
        {
            _logger.LogWarning("User not found");
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        _logger.LogInformation("Changing role into new role");
        var roleExists = await _context.Roles.AnyAsync(r => r.Name == changeRoleDto.NewRole);
        if (!roleExists)
        {
            _logger.LogWarning("Chosen role doesnt exist");
            return new Response<string>(HttpStatusCode.BadRequest, "Role does not exist");
        }

        var currentRoles = await _userManager.GetRolesAsync(user);

        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            _logger.LogError("Failed to remove existing roles");
            return new Response<string>(HttpStatusCode.InternalServerError, "Failed to remove existing roles");
        }

        var addResult = await _userManager.AddToRoleAsync(user, changeRoleDto.NewRole);
        if (!addResult.Succeeded)
        {
            _logger.LogError("Failed to add new role");
            return new Response<string>(HttpStatusCode.InternalServerError, "Failed to add new role");
        }

        return new Response<string>("Role changed successfully");
    }
}
