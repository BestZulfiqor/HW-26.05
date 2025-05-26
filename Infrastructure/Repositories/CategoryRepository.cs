using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;

namespace Infrastructure.Repositories;

public class CategoryRepository(DataContext context, IMapper mapper) : IBaseRepository<Category, int>
{
    public async Task<int> AddAsync(Category entity)
    {
        await context.Categories.AddAsync(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(Category entity)
    {
        var existing = await context.Categories.FindAsync(entity.Id);
        if (existing == null)
            throw new KeyNotFoundException($"Category with Id {entity.Id} not found.");

        mapper.Map(entity, existing);

        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(Category entity)
    {
        var existing = await context.Categories.FindAsync(entity.Id);
        if (existing == null)
            throw new KeyNotFoundException($"Category with Id {entity.Id} not found.");

        context.Categories.Remove(existing);
        return await context.SaveChangesAsync();
    }

    public Task<IQueryable<Category>> GetAllAsync()
    {
        return Task.FromResult(context.Categories.AsQueryable());
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await context.Categories.FindAsync(id);
    }

}