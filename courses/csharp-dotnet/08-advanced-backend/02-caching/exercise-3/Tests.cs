using Microsoft.Extensions.Caching.Memory;
using Xunit;

public class RateCacheTests
{
    private int _calls;
    private RateCache Make(TimeSpan ttl) => new(new MemoryCache(new MemoryCacheOptions()), () => ++_calls, ttl);

    [Fact] public void WithinTtl_FetchesOnce()
    {
        var cache = Make(TimeSpan.FromMinutes(1));
        cache.GetRate();
        Assert.Equal(1m, cache.GetRate());
    }
    [Fact] public async Task AfterTtl_FetchesAgain()
    {
        var cache = Make(TimeSpan.FromMilliseconds(50));
        cache.GetRate();
        await Task.Delay(150);
        Assert.Equal(2m, cache.GetRate());
    }
    [Fact] public void TryGet_NeverFetches()
    {
        Assert.Null(Make(TimeSpan.FromMinutes(1)).TryGetCached());
        Assert.Equal(0, _calls);
    }
    [Fact] public void TryGet_SeesCached()
    {
        var cache = Make(TimeSpan.FromMinutes(1));
        cache.GetRate();
        Assert.Equal(1m, cache.TryGetCached());
    }
}
