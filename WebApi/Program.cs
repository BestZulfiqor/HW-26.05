using Domain.DTOs.Seeder;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.AutoMapper;
using Infrastructure.BackgroundTasks;
using Infrastructure.Data;
using Infrastructure.DI;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(InfrastructureProfile));
builder.Services.AddLogging();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddHostedService<PremiumProductStatusUpdater>();
builder.Services.AddInfrastructure();
builder.Services.AddAuthenticationConfiguration(builder.Configuration);
builder.Services.AddDbContext<DataContext>(opt =>
    opt.UseNpgsql(connection));

builder.Services.AddMemoryCache(); // Регистрирует IMemoryCache, RedisCache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisCache");
    options.InstanceName = "SomonTJ";
});
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
    })
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.AddHangfire(opt => opt.UsePostgreSqlStorage(connection));
builder.Services.AddHangfireServer();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

await using var scope = app.Services.CreateAsyncScope();
var services = scope.ServiceProvider;
var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
await DefaultRoles.SeedAsync(roleManager);
await DefaultUser.SeedAsync(userManager);

app.Services.RegisterRecurringJobs();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseHangfireDashboard();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();