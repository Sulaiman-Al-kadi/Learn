## Hint 1
Constructor: `private readonly IMemoryCache _cache; private readonly ProductRepository _repository; public CachedProductService(IMemoryCache cache, ProductRepository repository) { _cache = cache; _repository = repository; }`

## Hint 2
```csharp
public List<Product> GetAllProducts()
{
    if (_cache.TryGetValue("all_products", out List<Product>? cached)) return cached!;
    List<Product> products = _repository.GetAll();
    _cache.Set("all_products", products, TimeSpan.FromMinutes(10));
    return products;
}
```

## Hint 3
`InvalidateCache`: `_cache.Remove("all_products");`. Use the exact same string key everywhere — a typo in the key string is a classic caching bug (consider a `private const string CacheKey = "all_products";` to avoid repeating the literal).
