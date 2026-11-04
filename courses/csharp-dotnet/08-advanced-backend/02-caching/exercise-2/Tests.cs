using Microsoft.Extensions.Caching.Memory;
using Xunit;

public class PriceCacheTests
{
    private int _calls;
    private readonly PriceCache _cache;

    public PriceCacheTests() => _cache = new PriceCache(new MemoryCache(new MemoryCacheOptions()), id => { _calls++; return Task.FromResult(id * 2m); });

    [Fact] public async Task Second_Read_IsCached()
    {
        await _cache.GetAsync(1);
        Assert.Equal(2m, await _cache.GetAsync(1));
        Assert.Equal(1, _calls);
    }
    [Fact] public async Task Keys_AreSeparate()
    {
        Assert.Equal(2m, await _cache.GetAsync(1));
        Assert.Equal(4m, await _cache.GetAsync(2));
        Assert.Equal(2, _calls);
    }
    [Fact] public async Task Invalidate_OnlyThatKey()
    {
        await _cache.GetAsync(1);
        await _cache.GetAsync(2);
        _cache.Invalidate(1);
        await _cache.GetAsync(1);
        await _cache.GetAsync(2);
        Assert.Equal(3, _calls);
    }
}
