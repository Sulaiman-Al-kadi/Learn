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

// TODO: see README.md

public partial class Program { }
