using Microsoft.AspNetCore.Mvc;

namespace Product.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly List<Product> Products = new()
    {
        new() { Id = 1, CategoryId = 1, Title = "Laptop", Price = 2500 },
        new() { Id = 2, CategoryId = 2, Title = "Monitor", Price = 1500 },
        new() { Id = 3, CategoryId = 3, Title = "Keyboard", Price = 500 },
    };

    [HttpGet]
    public IEnumerable<Product> Get() => Products;

    [HttpGet("{id}")]
    public Product? Get(int id) => Products.FirstOrDefault(f => f.Id == id);
}

public class Product
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
}