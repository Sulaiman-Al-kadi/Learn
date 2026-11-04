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
        throw new NotImplementedException();
    }

    public void Invalidate(int id)
    {
        throw new NotImplementedException();
    }
}
