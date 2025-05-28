using Microsoft.Extensions.DependencyInjection;
using Domain.Entities;
using Infrastructure.BackgroundTasks;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.DI;

public static class DependencyInjection
{
   public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IBaseRepository<Order, int>, OrderRepository>();
        
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IBaseRepository<Product, int>, ProductRepository>();
        
        services.AddScoped<ICategoriesService, CategoryService>();
        services.AddScoped<IBaseRepository<Category, int>, CategoryRepository>();
        
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPasswordHasher<Order>, PasswordHasher<Order>>();
        
        // Program.cs
        services.AddHostedService<PremiumProductStatusUpdater>();
        
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMemoryCacheService, MemoryCacheService>();
        services.AddScoped<IRedisCacheService, RedisCacheService>();
    }
}