using Domain.DTOs.Orders;
using Domain.Filters;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IOrderService
{
    Task<Response<GetOrderDto>> CreateOrderAsync(CreateOrderDto create);
    Task<Response<GetOrderDto>> UpdateOrderAsync(int id, UpdateOrderDto update);
    Task<Response<string>> DeleteOrder(int id);
    Task<Response<GetOrderDto>> GetByIdAsync(int id);
    Task<PagedResponse<List<GetOrderDto>>> GetAllAsync(OrderFilter filter);
}