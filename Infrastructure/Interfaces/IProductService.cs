using Domain.DTOs.Products;
using Domain.Filters;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IProductService
{
    Task<Response<GetProductDto>> CreateProductAsync(CreateProductDto create);
    Task<Response<GetProductDto>> UpdateProductAsync(int id, UpdateProductDto update);
    Task<Response<string>> DeleteProduct(int id);
    Task<Response<GetProductDto>> GetByIdAsync(int id);
    Task<PagedResponse<List<GetProductDto>>> GetAllAsync(ProductFilter filter);
}