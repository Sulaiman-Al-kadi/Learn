// Exercise 02 — CachedProductService
// See README.md.
using Microsoft.Extensions.Caching.Memory;

public record Product(int Id, string Name, decimal Price);

public class ProductRepository
{
    public int CallCount { get; private set; }
    private readonly List<Product> _products = new List<Product>
    {
        new Product(1, "Widget", 9.99m),
        new Product(2, "Gadget", 19.99m),
    };

    public List<Product> GetAll()
    {
        CallCount++;
        return _products;
    }
}

public class CachedProductService
{
    // TODO: private readonly IMemoryCache field, private readonly ProductRepository field — set via constructor
    public CachedProductService(IMemoryCache cache, ProductRepository repository)
    {
        throw new NotImplementedException();
    }

    public List<Product> GetAllProducts()
    {
        throw new NotImplementedException();
    }

    public void InvalidateCache()
    {
        throw new NotImplementedException();
    }
}
