using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundTasks;

public class PremiumProductStatusUpdater : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<PremiumProductStatusUpdater> _logger;

    public PremiumProductStatusUpdater(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<PremiumProductStatusUpdater> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Служба проверки Premium статусов запущена");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                
                var expiredProducts = await dbContext.Products
                    .Where(p => (p.IsPremium || p.IsTop) && 
                               p.PremiumOrTopExpiryDate < DateTimeOffset.UtcNow)
                    .ToListAsync(stoppingToken);

                if (expiredProducts.Any())
                {
                    foreach (var product in expiredProducts)
                    {
                        product.IsPremium = false;
                        product.IsTop = false;
                        _logger.LogInformation($"Сброшен Premium статус для продукта {product.Id}");
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation($"Обновлено {expiredProducts.Count} продуктов");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в фоновой службе");
            }

            await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
        }
    }
}