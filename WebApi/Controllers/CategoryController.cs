using Domain.Constants;
using Domain.DTOs.Categories;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoriesService service)
{
    [HttpGet]
    public async Task<Response<List<GetCategoryDto>>> GetAllAsync([FromQuery]CategoryFilter filter)
    {
        var user = await service.GetAllAsync(filter);
        return user;
    }
    
    [HttpGet("{id}")]
    public async Task<Response<GetCategoryDto>> GetAsync(int id)
    {
        var user = await service.GetByIdAsync(id);
        return user;
    }

    [HttpPost]
    public async Task<Response<GetCategoryDto>> Create(CreateCategoryDto request)
    {
        var response = await service.CreateCategoriesAsync(request);
        return response;
    }

    [HttpPut("{id}")]
    public async Task<Response<GetCategoryDto>> Update(int id, UpdateCategoryDto request)
    {
        var response = await service.UpdateCategoriesAsync(id, request);
        return response;
    }

    [HttpDelete("{id:int}")]
    public async Task<Response<string>> Delete(int id)
    {
        var response = await service.DeleteCategories(id);
        return response;
    }
}