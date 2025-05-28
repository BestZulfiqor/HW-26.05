using Domain.DTOs.Auth;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<Response<string>> Register(RegisterDto registerDto)
    {
        return await authService.Register(registerDto);
    }
    
    [HttpPost("login")]
    public async Task<Response<TokenDto>> Login(LoginDto loginDto)
    {
        return await authService.Login(loginDto);
    }
    
    [HttpPost("reset-password")]
    public async Task<Response<string>> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        return await authService.ResetPassword(resetPasswordDto);
    }
    
    [AllowAnonymous]
    [HttpPost("request-reset-password")]
    public async Task<Response<string>> RequestResetPassword(RequestResetPassword resetPasswordDto)
    {
        return await authService.RequestResetPassword(resetPasswordDto);
    }

    [HttpPut("{changeRoleDto}")]
    public async Task<Response<string>> ChangeRole(ChangeRoleDto changeRoleDto) =>
        await authService.ChangeRole(changeRoleDto);
}