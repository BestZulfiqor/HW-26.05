using System.Net;
using AutoMapper;
using Domain.Constants;
using Domain.DTOs.Users;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UserService(UserManager<IdentityUser> userManager, IMapper mapper) : IUserService
{
    public async Task<Response<List<GetUserDto>>> GetAllAsync()
    {
        var users = await userManager.Users.ToListAsync();

        var result = new List<GetUserDto>();
        foreach (var user in users)
        {
            var dto = mapper.Map<GetUserDto>(user);
            dto.Roles = (await userManager.GetRolesAsync(user)).ToList();
            result.Add(dto);
        }

        return new Response<List<GetUserDto>>(result);
    }

    public async Task<Response<GetUserDto>> GetByIdAsync(string id)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            return new Response<GetUserDto>(HttpStatusCode.NotFound, "User not found");

        var dto = mapper.Map<GetUserDto>(user);
        dto.Roles = (await userManager.GetRolesAsync(user)).ToList();

        return new Response<GetUserDto>(dto);
    }

    public async Task<Response<GetUserDto>> CreateAsync(CreateUserDto request)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            return new Response<GetUserDto>(HttpStatusCode.Conflict, "Email already in use");
        
        var user = new IdentityUser
        {
            UserName = request.Username,
            Email = request.Email
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
            return new Response<GetUserDto>(HttpStatusCode.BadRequest, errorMessages);
        }

        await userManager.AddToRoleAsync(user, Roles.User);

        var dto = mapper.Map<GetUserDto>(user);
        dto.Roles = (await userManager.GetRolesAsync(user)).ToList();
        return new Response<GetUserDto>(dto);
    }

    public async Task<Response<GetUserDto>> UpdateAsync(string id, UpdateUserDto request)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            return new Response<GetUserDto>(HttpStatusCode.NotFound, "User not found");

        user.UserName = request.Username;
        user.Email = request.Email;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
            return new Response<GetUserDto>(HttpStatusCode.BadRequest, errorMessages);
        }

        var dto = mapper.Map<GetUserDto>(user);
        dto.Roles = (await userManager.GetRolesAsync(user)).ToList();
        return new Response<GetUserDto>(dto);
    }

    public async Task<Response<string>> DeleteAsync(string id)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            return new Response<string>(HttpStatusCode.NotFound, "User not found");

        var result = await userManager.DeleteAsync(user);
        return result.Succeeded
            ? new Response<string>("User deleted")
            : new Response<string>(HttpStatusCode.BadRequest, "Failed to delete user");
    }
}
