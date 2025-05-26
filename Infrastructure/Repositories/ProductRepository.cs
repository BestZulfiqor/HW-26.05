using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;

namespace Infrastructure.Repositories;

public class ProductRepository(DataContext context, IMapper mapper) : IBaseRepository<Product, int>
{
    public async Task<int> AddAsync(Product entity)
    {
        await context.Products.AddAsync(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(Product entity)
    {
        var existing = await context.Products.FindAsync(entity.Id);
        if (existing == null)
            throw new KeyNotFoundException($"Product with Id {entity.Id} not found.");

        mapper.Map(entity, existing);

        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(Product entity)
    {
        var existing = await context.Products.FindAsync(entity.Id);
        if (existing == null)
            throw new KeyNotFoundException($"Product with Id {entity.Id} not found.");

        context.Products.Remove(existing);
        return await context.SaveChangesAsync();
    }

    public Task<IQueryable<Product>> GetAllAsync()
    {
        return Task.FromResult(context.Products.AsQueryable());
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await context.Products.FindAsync(id);
    }

}