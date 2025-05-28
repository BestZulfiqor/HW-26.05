using Domain.DTOs.Users;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IUserService
{
    Task<Response<List<GetUserDto>>> GetAllAsync();
    Task<Response<GetUserDto>> GetByIdAsync(string id);
    Task<Response<GetUserDto>> CreateAsync(CreateUserDto request);
    Task<Response<GetUserDto>> UpdateAsync(string id, UpdateUserDto request);
    Task<Response<string>> DeleteAsync(string id);
}