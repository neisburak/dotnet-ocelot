
using Microsoft.AspNetCore.Mvc;

namespace Discount.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class DiscountController : ControllerBase
{
    private readonly List<Discount> Discounts = new()
    {
        new() { Id = 1, ProductId = 1, Amount = 150, ValidUntil = DateTime.UtcNow.AddDays(1) },
        new() { Id = 2, ProductId = 3, Amount = 50, ValidUntil = DateTime.UtcNow.AddDays(10) },
    };

    [HttpGet("{id}")]
    public Discount? Get(int id) => Discounts.FirstOrDefault(f => f.ProductId == id);
}

public class Discount
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public decimal Amount { get; set; }
    public DateTime ValidUntil { get; set; }
}
