using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;

namespace Infrastructure.Repositories;

public class OrderRepository(DataContext context, IMapper mapper) : IBaseRepository<Order, int>
{
    public async Task<int> AddAsync(Order entity)
    {
        await context.Orders.AddAsync(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(Order entity)
    {
        var existing = await context.Orders.FindAsync(entity.Id);
        if (existing == null)
            throw new KeyNotFoundException($"Order with Id {entity.Id} not found.");

        mapper.Map(entity, existing);

        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(Order entity)
    {
        var existing = await context.Orders.FindAsync(entity.Id);
        if (existing == null)
            throw new KeyNotFoundException($"Order with Id {entity.Id} not found.");

        context.Orders.Remove(existing);
        return await context.SaveChangesAsync();
    }

    public Task<IQueryable<Order>> GetAllAsync()
    {
        return Task.FromResult(context.Orders.AsQueryable());
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await context.Orders.FindAsync(id);
    }

}