using System.Net;
using AutoMapper;
using Domain.DTOs.Orders;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class OrderService(IBaseRepository<Order, int> orderRepository, IMapper mapper) : IOrderService
{
    public async Task<Response<GetOrderDto>> CreateOrderAsync(CreateOrderDto create)
    {
        var order = new Order()
        {
            OrderDate = create.OrderDate,
            ProductId = create.ProductId,
            Quantity = create.Quantity,
            UserId = create.UserId,
            Status= create.Status,
        };

        var result = await orderRepository.AddAsync(order);

        if (result == 0)
        {
            return new Response<GetOrderDto>(HttpStatusCode.BadRequest, "Order not created");
        }

        var dto = mapper.Map<GetOrderDto>(order);
        return new Response<GetOrderDto>(dto);
    }

    public async Task<Response<GetOrderDto>> UpdateOrderAsync(int id, UpdateOrderDto update)
    {
        var order = await orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            return new Response<GetOrderDto>(HttpStatusCode.NotFound, "Order not found");
        }

        mapper.Map(update, order);

        var result = await orderRepository.UpdateAsync(order);
        if (result == 0)
        {
            return new Response<GetOrderDto>(HttpStatusCode.BadRequest, "Order not updated");
        }

        var dto = mapper.Map<GetOrderDto>(order);
        return new Response<GetOrderDto>(dto);
    }

    public async Task<Response<string>> DeleteOrder(int id)
    {
        var order = await orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Order not found");
        }

        var result = await orderRepository.DeleteAsync(order);
        if (result == 0)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Order not deleted");
        }

        return new Response<string>("Order deleted successfully");
    }

    public async Task<PagedResponse<List<GetOrderDto>>> GetAllAsync(OrderFilter filter)
    {
        var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
        var pageSize = filter.PageSize < 10 ? 10 : filter.PageSize;

        var query = await orderRepository.GetAllAsync();
        var totalRecords = await query.CountAsync();

        var pagedOrders = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = mapper.Map<List<GetOrderDto>>(pagedOrders);

        return new PagedResponse<List<GetOrderDto>>(dtos, pageNumber, pageSize, totalRecords);
    }

    public async Task<Response<GetOrderDto>> GetByIdAsync(int id)
    {
        var order = await orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            return new Response<GetOrderDto>(HttpStatusCode.NotFound, "Order not found");
        }

        var dto = mapper.Map<GetOrderDto>(order);
        return new Response<GetOrderDto>(dto);
    }
}