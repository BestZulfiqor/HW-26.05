using AutoMapper;
using Domain.DTOs;
using Domain.DTOs.Categories;
using Domain.DTOs.Orders;
using Domain.DTOs.Products;
using Domain.DTOs.Users;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.AutoMapper;

public class InfrastructureProfile : Profile
{
    public InfrastructureProfile()
    { 
        CreateMap<Order, GetOrderDto>();
        CreateMap<CreateOrderDto, Order>();
        CreateMap<UpdateOrderDto, Order>();
        
        CreateMap<Product, GetProductDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
        
        CreateMap<Category, GetCategoryDto>();
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();
        
        CreateMap<IdentityUser, GetUserDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName));

    }
}