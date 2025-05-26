using Domain.DTOs.Users;
using Domain.Filters;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IUserService
{
    Task<Response<GetUserDto>> CreateUserAsync(GetUserDto create);
    Task<Response<GetUserDto>> UpdateUserAsync(int id, GetUserDto update);
    Task<Response<string>> DeleteUser(int id);
    Task<Response<GetUserDto>> GetByIdAsync(int id);
    Task<PagedResponse<List<GetUserDto>>> GetAllAsync(UserFilter filter);
}