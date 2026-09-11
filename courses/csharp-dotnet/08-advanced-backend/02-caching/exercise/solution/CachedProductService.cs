// Exercise 02 — CachedProductService (solution)
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
    private const string CacheKey = "all_products";
    private readonly IMemoryCache _cache;
    private readonly ProductRepository _repository;

    public CachedProductService(IMemoryCache cache, ProductRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }

    public List<Product> GetAllProducts()
    {
        if (_cache.TryGetValue(CacheKey, out List<Product>? cached))
        {
            return cached!;
        }

        List<Product> products = _repository.GetAll();
        _cache.Set(CacheKey, products, TimeSpan.FromMinutes(10));
        return products;
    }

    public void InvalidateCache()
    {
        _cache.Remove(CacheKey);
    }
}
