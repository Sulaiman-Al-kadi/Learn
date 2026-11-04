using System.Diagnostics;
using Xunit;

public class FetcherTests
{
    private static async Task<int> Slow(int id) { await Task.Delay(200); return id * 10; }

    [Fact] public async Task Sum_AddsResults() => Assert.Equal(60, await Fetcher.SumAllAsync([1, 2, 3], Slow));
    [Fact] public async Task Sum_RunsConcurrently()
    {
        var clock = Stopwatch.StartNew();
        await Fetcher.SumAllAsync([1, 2, 3, 4, 5], Slow);
        Assert.True(clock.ElapsedMilliseconds < 700, "The calls ran one after another.");
    }
    [Fact] public async Task InOrder_KeepsOrder()
    {
        async Task<int> Reversed(int id) { await Task.Delay(100 - id * 30); return id; }
        Assert.Equal([1, 2, 3], await Fetcher.FetchInOrderAsync([1, 2, 3], Reversed));
    }
}
