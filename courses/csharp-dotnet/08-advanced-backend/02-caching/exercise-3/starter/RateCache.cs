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
        throw new NotImplementedException();
    }

    public decimal? TryGetCached()
    {
        throw new NotImplementedException();
    }
}
