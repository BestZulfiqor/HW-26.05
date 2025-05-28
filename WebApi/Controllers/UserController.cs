using Domain.Constants;
using Domain.DTOs.Users;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<Response<List<GetUserDto>>> GetAllAsync()
    {
        return await _service.GetAllAsync();
    }

    [HttpGet("{id}")]
    public async Task<Response<GetUserDto>> GetByIdAsync(string id)
    {
        return await _service.GetByIdAsync(id);
    }

    [HttpPost]
    public async Task<Response<GetUserDto>> Create(CreateUserDto request)
    {
        return await _service.CreateAsync(request);
    }

    [HttpPut("{id}")]
    public async Task<Response<GetUserDto>> Update(string id, UpdateUserDto request)
    {
        return await _service.UpdateAsync(id, request);
    }

    [HttpDelete("{id}")]
    public async Task<Response<string>> Delete(string id)
    {
        return await _service.DeleteAsync(id);
    }
}