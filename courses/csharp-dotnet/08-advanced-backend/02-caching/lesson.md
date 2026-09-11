# Module 8, Lesson 02 — Caching

Module 6's EF Core queries hit a real database every time — correct, but not free. If the same data is requested repeatedly and doesn't change often (a list of countries, a product catalog, a computed report), re-querying every single time wastes time and database load for no benefit. **Caching** stores a computed result temporarily so the next request can reuse it instead of redoing the work.

## `IMemoryCache` — an in-process cache

```csharp
builder.Services.AddMemoryCache();
```

```csharp
public class ProductService
{
    private readonly IMemoryCache _cache;
    private readonly AppDbContext _context;

    public ProductService(IMemoryCache cache, AppDbContext context)
    {
        _cache = cache;
        _context = context;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        if (_cache.TryGetValue("all_products", out List<Product>? cached))
        {
            return cached!;                                    // cache hit — skip the database entirely
        }

        List<Product> products = await _context.Products.ToListAsync();     // cache miss — do the real work
        _cache.Set("all_products", products, TimeSpan.FromMinutes(10));      // remember it for next time
        return products;
    }
}
```

| Piece | Meaning |
|---|---|
| `_cache.TryGetValue(key, out value)` | Checks whether something is already cached under `key`, without throwing if it's missing (Module 1, lesson 09's `TryParse` philosophy, again). |
| `_cache.Set(key, value, expiration)` | Stores a value, with a lifetime — after `10` minutes it's automatically evicted and the next request becomes a cache miss again. |

This pattern — check the cache, and only do the expensive work on a miss — is called **cache-aside**, and it's by far the most common caching pattern you'll use.

## Choosing a cache duration and key

- **Duration**: how quickly is this data allowed to go stale? A product catalog that changes rarely might cache for hours; a stock price needs seconds, if it's cached at all.
- **Key**: must uniquely identify what's cached. `"all_products"` works for one global list; per-item lookups need the id baked in: `$"product_{id}"`.

```csharp
public async Task<Product?> GetProductByIdAsync(int id)
{
    string key = $"product_{id}";
    if (_cache.TryGetValue(key, out Product? cached)) return cached;

    Product? product = await _context.Products.FindAsync(id);
    if (product is not null)
    {
        _cache.Set(key, product, TimeSpan.FromMinutes(5));
    }
    return product;
}
```

## `GetOrCreateAsync` — the cache-aside pattern in one call

`IMemoryCache` provides a shortcut combining the check-and-populate steps:

```csharp
public async Task<List<Product>> GetAllProductsAsync()
{
    return await _cache.GetOrCreateAsync("all_products", async entry =>
    {
        entry.SlidingExpiration = TimeSpan.FromMinutes(10);
        return await _context.Products.ToListAsync();
    }) ?? [];
}
```

`GetOrCreateAsync` checks the cache; on a miss, it runs your lambda, stores the result, and returns it — all in one call, instead of writing the `TryGetValue`/`Set` pair by hand every time.

## Invalidating the cache — the hard part

The classic saying: "there are only two hard problems in computer science: cache invalidation, naming things, and off-by-one errors." When underlying data **changes**, a stale cache entry becomes actively wrong, not just slow:

```csharp
public async Task<Product> CreateProductAsync(string name, decimal price)
{
    var product = new Product { Name = name, Price = price };
    _context.Products.Add(product);
    await _context.SaveChangesAsync();

    _cache.Remove("all_products");        // the cached list is now stale — evict it
    return product;
}
```

Whenever you write data that a cache entry depends on, you must explicitly remove (or update) that entry — the cache has no way to know your database changed underneath it.

## When NOT to cache

- Data that changes on every request (there's nothing to reuse).
- Data specific to one user, cached under a shared key (a real, serious bug — one user could see another's data).
- Anything where staleness is unacceptable (a bank balance, an inventory count right before checkout).

Caching is a performance optimization with a real correctness cost if used carelessly — reach for it deliberately, not by default.

## Summary
- `IMemoryCache`: an in-process cache, registered with `AddMemoryCache()`.
- **Cache-aside**: check the cache first; on a miss, do the real work and store the result.
- `TryGetValue`/`Set`, or the combined `GetOrCreateAsync`, are the two ways to write this pattern.
- Cache **duration** should match how quickly the data is allowed to go stale; the **key** must uniquely identify what's cached.
- **Invalidate** (`Remove`) cached entries when the underlying data changes — the cache doesn't know on its own.
- Don't cache per-request-unique or user-specific data under a shared key, and don't cache data where staleness is unacceptable.
