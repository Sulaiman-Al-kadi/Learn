# Exercise — CachedProductService

Write everything in `CachedProductService.cs`. `ProductRepository` (given below, already in the starter — don't change it) simulates a slow data source and counts how many times it was actually called, so the tests can verify caching is really happening.

```csharp
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
```

## `CachedProductService`
Constructor takes an `IMemoryCache` and a `ProductRepository` (both injected).

| Method | Does |
|---|---|
| `List<Product> GetAllProducts()` | cache-aside: check the cache under key `"all_products"`; on a hit, return the cached value. On a miss, call `_repository.GetAll()`, cache the result for 10 minutes, and return it. |
| `void InvalidateCache()` | removes the `"all_products"` entry from the cache |

## Rules
- `GetAllProducts` must **never** call `_repository.GetAll()` on a cache hit — that's the entire point of caching, and it's exactly what the tests check via `ProductRepository.CallCount`.
- Use `_cache.TryGetValue(...)` and `_cache.Set(...)` (not `GetOrCreateAsync` — this method isn't async).
