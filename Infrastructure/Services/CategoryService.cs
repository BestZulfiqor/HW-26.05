using System.Net;
using AutoMapper;
using Domain.DTOs.Categories;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class CategoryService(IBaseRepository<Category, int> categoryRepository, IMapper mapper) : ICategoriesService
{
    public async Task<PagedResponse<List<GetCategoryDto>>> GetAllAsync(CategoryFilter filter)
    {
        var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
        var pageSize = filter.PageSize < 10 ? 10 : filter.PageSize;

        var query = await categoryRepository.GetAllAsync();
        var totalRecords = await query.CountAsync();

        var pagedCategories = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = mapper.Map<List<GetCategoryDto>>(pagedCategories);

        return new PagedResponse<List<GetCategoryDto>>(dtos, pageNumber, pageSize, totalRecords);
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