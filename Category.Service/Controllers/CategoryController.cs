using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Category.Service.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    private readonly List<Category> Categories = new()
    {
        new() { Id = 1, Title = "Laptop" },
        new() { Id = 2, Title = "Monitor" },
        new() { Id = 3, Title = "Keyboard" },
    };

    [HttpGet]
    public IEnumerable<Category> Get() => Categories;
}

public class Category
{
    public int Id { get; set; }
    public string Title { get; set; }
}
