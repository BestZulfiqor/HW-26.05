using Domain.DTOs;
using Domain.DTOs.Email;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.BackgroundTasks;

public class SendPaymentEmailWithHangFire(
    IServiceScopeFactory serviceScopeFactory)
{
    public async Task Send()
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var orderRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<Order, int>>();

        var orders = await dataContext.Orders.ToListAsync();

        var now = DateTime.UtcNow;

        foreach (var order in orders)
        {
            if (order.OrderDate < DateTime.Now)
            {
                var updateOrder = order;

                await orderRepository.UpdateAsync(updateOrder);

                var emailDto = new EmailDto()
                {
                    To = order.User.Email,
                    Subject = "Delivery overdue",
                    Body = $"Sorry, your order is beign late.\n\n A discount of 20% has been applied to your order"
                };

                await emailService.SendEmailAsync(emailDto);
            }
            else
            {
                var emailDto = new EmailDto()
                {
                    To = order.User.Email,
                    Subject = "Delivery info",
                    Body = $"Your order is on the way."
                };

                await emailService.SendEmailAsync(emailDto);
            }
        }
    }
}