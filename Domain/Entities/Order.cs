using Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    public Status Status { get; set; }

    public List<Product> Products { get; set; }
    public IdentityUser User { get; set; }
}