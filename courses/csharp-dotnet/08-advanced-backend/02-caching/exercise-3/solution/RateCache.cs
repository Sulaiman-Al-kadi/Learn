using Microsoft.Extensions.Caching.Memory;

public class RateCache
{
    private readonly IMemoryCache _cache;
    private readonly Func<decimal> _fetch;
    private readonly TimeSpan _ttl;

    public RateCache(IMemoryCache cache, Func<decimal> fetch, TimeSpan ttl)
    {
        _cache = cache;
        _fetch = fetch;
        _ttl = ttl;
    }

    public decimal GetRate()
    {
        if (_cache.TryGetValue("rate", out decimal cached)) return cached;
        decimal fresh = _fetch();
        _cache.Set("rate", fresh, _ttl);
        return fresh;
    }

    public decimal? TryGetCached()
    {
        return _cache.TryGetValue("rate", out decimal cached) ? cached : null;
    }
}
