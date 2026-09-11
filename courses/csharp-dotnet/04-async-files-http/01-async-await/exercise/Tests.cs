using System.Diagnostics;
using Xunit;

public class AsyncToolsTests
{
    [Fact]
    public async Task SquareAfterDelayAsync_ReturnsSquare()
    {
        int result = await AsyncTools.SquareAfterDelayAsync(5, 10);
        Assert.Equal(25, result);
    }

    [Fact]
    public async Task SquareAllAsync_ReturnsSquaresInOrder()
    {
        var result = await AsyncTools.SquareAllAsync(new List<int> { 2, 3, 4 }, 10);
        Assert.Equal(new List<int> { 4, 9, 16 }, result);
    }

    [Fact]
    public async Task SquareAllAsync_RunsConcurrently()
    {
        var sw = Stopwatch.StartNew();
        await AsyncTools.SquareAllAsync(new List<int> { 1, 2, 3 }, 60);
        sw.Stop();

        // Sequential would take ~180ms+; concurrent should take well under that.
        Assert.True(sw.ElapsedMilliseconds < 150, $"Took {sw.ElapsedMilliseconds}ms — looks sequential, not concurrent.");
    }

    [Fact]
    public async Task SafeDivideAsync_NormalCase_ReturnsQuotient()
    {
        int result = await AsyncTools.SafeDivideAsync(10, 2);
        Assert.Equal(5, result);
    }

    [Fact]
    public async Task SafeDivideAsync_ByZero_Throws()
    {
        await Assert.ThrowsAsync<DivideByZeroException>(() => AsyncTools.SafeDivideAsync(10, 0));
    }
}
