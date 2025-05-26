using Domain.DTOs.Orders;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IOrderService orderService) : ControllerBase
{
    [HttpGet]
    public async Task<PagedResponse<List<GetOrderDto>>> GetAllAsync([FromQuery] OrderFilter filter)
    {
        return await orderService.GetAllAsync(filter);
    }
    
    [HttpGet("{id}")]
    public async Task<Response<GetOrderDto>> GetByIdAsync(int id)
    {
        return await orderService.GetByIdAsync(id);
    }
    
    [HttpPost]
    public async Task<Response<GetOrderDto>> CreateOrderAsync(CreateOrderDto create)
    {
        return await orderService.CreateOrderAsync(create);
    }
    
    [HttpPut]
    public async Task<Response<GetOrderDto>> UpdateOrderAsync(int id, UpdateOrderDto update)
    {
        return await orderService.UpdateOrderAsync(id, update);
    }
    
    [HttpDelete]
    public async Task<Response<string>> DeleteOrder(int id)
    {
        return await orderService.DeleteOrder(id);
    }
}