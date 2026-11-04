using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IProductStore, ProductStore>();
builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();

app.Run();

public record Product(int Id, string Name);

public interface IProductStore
{
    List<Product> All();
    Product Add(string name);
}

public class ProductStore : IProductStore
{
    private readonly List<Product> _items = [new(1, "Pen")];
    public List<Product> All() => _items;
    public Product Add(string name)
    {
        var p = new Product(_items.Count + 1, name);
        _items.Add(p);
        return p;
    }
}

public record NewProduct(string Name);

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly IProductStore _store;

    public ProductsController(IProductStore store) => _store = store;

    [HttpGet]
    public List<Product> GetAll() => _store.All();

    [HttpGet("{id}")]
    public ActionResult<Product> GetOne(int id)
    {
        var product = _store.All().FirstOrDefault(p => p.Id == id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public ActionResult<Product> Create(NewProduct body)
    {
        var product = _store.Add(body.Name);
        return CreatedAtAction(nameof(GetOne), new { id = product.Id }, product);
    }
}

public partial class Program { }
