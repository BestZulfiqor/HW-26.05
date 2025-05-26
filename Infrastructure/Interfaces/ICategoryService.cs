using Domain.DTOs.Categories;
using Domain.Filters;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface ICategoriesService
{
    Task<Response<GetCategoryDto>> CreateCategoriesAsync(CreateCategoryDto create);
    Task<Response<GetCategoryDto>> UpdateCategoriesAsync(int id, UpdateCategoryDto update);
    Task<Response<string>> DeleteCategories(int id);
    Task<Response<GetCategoryDto>> GetByIdAsync(int id);
    Task<PagedResponse<List<GetCategoryDto>>> GetAllAsync(CategoryFilter filter);
}