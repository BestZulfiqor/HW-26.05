using Domain.Constants;
using Domain.DTOs.Products;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService service)
{
    [HttpGet]
    public async Task<Response<List<GetProductDto>>> GetAllAsync([FromQuery]ProductFilter filter)
    {
        var user = await service.GetAllAsync(filter);
        return user;
    }
    
    [HttpGet("{id}")]
    public async Task<Response<GetProductDto>> GetAsync(int id)
    {
        var user = await service.GetByIdAsync(id);
        return user;
    }

    [HttpPost]
    public async Task<Response<GetProductDto>> Create(CreateProductDto request)
    {
        var response = await service.CreateProductAsync(request);
        return response;
    }

    [HttpPut("{id}")]
    public async Task<Response<GetProductDto>> Update(int id, UpdateProductDto request)
    {
        var response = await service.UpdateProductAsync(id, request);
        return response;
    }

    [HttpDelete("{id:int}")]
    public async Task<Response<string>> Delete(int id)
    {
        var response = await service.DeleteProduct(id);
        return response;
    }
}