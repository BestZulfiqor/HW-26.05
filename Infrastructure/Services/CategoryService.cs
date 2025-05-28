using System.Net;
using AutoMapper;
using Domain.Constants;
using Domain.DTOs.Categories;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class CategoryService(
    IBaseRepository<Category, int> categoryRepository,
    IMapper mapper,
    IMemoryCacheService memoryCache,
    IRedisCacheService redisCache) : ICategoriesService
{
    public async Task<PagedResponse<List<GetCategoryDto>>> GetAllAsync(CategoryFilter filter)
    {
        // var categoryInCache = await memoryCache.GetData<List<GetCategoryDto>>(Cache.CategoryCache);
        var categoryInCache = await redisCache.GetData<List<GetCategoryDto>>(Cache.CategoryCache);

        if (categoryInCache == null)
        {
            var categories = await categoryRepository.GetAllAsync();
            categoryInCache = categories.Select(c => new GetCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
            }).ToList();

            // await memoryCache.SetData(Cache.CategoryCache, categoryInCache, 1);
            await redisCache.SetData(Cache.CategoryCache, categoryInCache, 1);
        }

        if (string.IsNullOrEmpty(filter.Name))
        {
            categoryInCache = categoryInCache.Where(n =>
                n.Name.ToLower().Trim().Contains(filter.Name!.ToLower().Trim())).ToList();
        }

        var totalCount = categoryInCache.Count;
        var paginationData = categoryInCache
            .Skip(filter.PageSize * (filter.PageNumber - 1))
            .Take(filter.PageSize)
            .ToList();

        return new PagedResponse<List<GetCategoryDto>>(paginationData, filter.PageSize, filter.PageNumber, totalCount);
    }

    public async Task<Response<GetCategoryDto>> CreateCategoriesAsync(CreateCategoryDto create)
    {
        var category = new Category()
        {
            Description = create.Description,
            Name = create.Name,
        };

        var result = await categoryRepository.AddAsync(category);

        if (result == 0)
        {
            return new Response<GetCategoryDto>(HttpStatusCode.BadRequest, "Category not created");
        }
        
        // await memoryCache.DeleteData(Cache.CategoryCache); 
        await redisCache.RemoveData(Cache.CategoryCache); 
        
        var dto = mapper.Map<GetCategoryDto>(category);
        return new Response<GetCategoryDto>(dto);
    }

    public async Task<Response<GetCategoryDto>> UpdateCategoriesAsync(int id, UpdateCategoryDto update)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return new Response<GetCategoryDto>(HttpStatusCode.NotFound, "Category not found");
        }

        mapper.Map(update, category);

        var result = await categoryRepository.UpdateAsync(category);
        if (result == 0)
        {
            return new Response<GetCategoryDto>(HttpStatusCode.BadRequest, "Category not updated");
        }
        
        // await memoryCache.DeleteData(Cache.CategoryCache); 
        await redisCache.RemoveData(Cache.CategoryCache); 
        
        var dto = mapper.Map<GetCategoryDto>(category);
        return new Response<GetCategoryDto>(dto);
    }

    public async Task<Response<string>> DeleteCategories(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Category not found");
        }

        var result = await categoryRepository.DeleteAsync(category);
        if (result == 0)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Category not deleted");
        }
        // await memoryCache.DeleteData(Cache.CategoryCache); 
        await redisCache.RemoveData(Cache.CategoryCache); 
        return new Response<string>("Category deleted successfully");
    }

    public async Task<Response<GetCategoryDto>> GetByIdAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return new Response<GetCategoryDto>(HttpStatusCode.NotFound, "Category not found");
        }

        var dto = mapper.Map<GetCategoryDto>(category);
        return new Response<GetCategoryDto>(dto);
    }
}