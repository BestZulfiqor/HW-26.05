using System.Net;
using AutoMapper;
using Domain.DTOs.Products;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ProductService(IBaseRepository<Product, int> productRepository, IMapper mapper) : IProductService
{
    public async Task<Response<GetProductDto>> CreateProductAsync(CreateProductDto create)
    {
        var product = new Product()
        {
            CategoryId = create.CategoryId,
            UserId = create.UserId,
            Description= create.Description,
            IsPremium = create.IsPremium,
            IsTop= create.IsTop,
            Name= create.Name,
            PremiumOrTopExpiryDate= create.PremiumOrTopExpiryDate,
            Price= create.Price,
        };

        var result = await productRepository.AddAsync(product);

        if (result == 0)
        {
            return new Response<GetProductDto>(HttpStatusCode.BadRequest, "Product not created");
        }

        var dto = mapper.Map<GetProductDto>(product);
        return new Response<GetProductDto>(dto);
    }

    public async Task<Response<GetProductDto>> UpdateProductAsync(int id, UpdateProductDto update)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return new Response<GetProductDto>(HttpStatusCode.NotFound, "Product not found");
        }

        mapper.Map(update, product);

        var result = await productRepository.UpdateAsync(product);
        if (result == 0)
        {
            return new Response<GetProductDto>(HttpStatusCode.BadRequest, "Product not updated");
        }

        var dto = mapper.Map<GetProductDto>(product);
        return new Response<GetProductDto>(dto);
    }

    public async Task<Response<string>> DeleteProduct(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Product not found");
        }

        var result = await productRepository.DeleteAsync(product);
        if (result == 0)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Product not deleted");
        }

        return new Response<string>("Product deleted successfully");
    }

    public async Task<PagedResponse<List<GetProductDto>>> GetAllAsync(ProductFilter filter)
    {
        var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
        var pageSize = filter.PageSize < 10 ? 10 : filter.PageSize;

        var query = await productRepository.GetAllAsync();
        var totalRecords = await query.CountAsync();

        var pagedProducts = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = mapper.Map<List<GetProductDto>>(pagedProducts);

        return new PagedResponse<List<GetProductDto>>(dtos, pageNumber, pageSize, totalRecords);
    }

    public async Task<Response<GetProductDto>> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return new Response<GetProductDto>(HttpStatusCode.NotFound, "Product not found");
        }

        var dto = mapper.Map<GetProductDto>(product);
        return new Response<GetProductDto>(dto);
    }
}