using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public int UserId { get; set; }
    public int OrderId { get; set; }
    public bool IsTop { get; set; }
    public bool IsPremium { get; set; }
    public DateTimeOffset PremiumOrTopExpiryDate { get; set; }

    public Category Category { get; set; }
    public IdentityUser User { get; set; }
    public Order Order { get; set; }
}