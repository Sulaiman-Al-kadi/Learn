using Microsoft.Extensions.Caching.Memory;

public class PriceCache
{
    private readonly IMemoryCache _cache;
    private readonly Func<int, Task<decimal>> _source;

    public PriceCache(IMemoryCache cache, Func<int, Task<decimal>> source)
    {
        _cache = cache;
        _source = source;
    }

    public async Task<decimal> GetAsync(int id)
    {
        return await _cache.GetOrCreateAsync($"price:{id}", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return _source(id);
        });
    }

    public void Invalidate(int id)
    {
        _cache.Remove($"price:{id}");
    }
}
