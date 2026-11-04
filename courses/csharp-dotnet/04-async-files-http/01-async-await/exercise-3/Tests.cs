using Xunit;

public class RetryTests
{
    private static Func<Task<int>> FailsTimes(int failures, Action? onCall = null)
    {
        int calls = 0;
        return async () =>
        {
            await Task.Yield();
            onCall?.Invoke();
            if (++calls <= failures) throw new InvalidOperationException($"fail {calls}");
            return 42;
        };
    }

    [Fact] public async Task Retry_SucceedsAfterFailures() => Assert.Equal(42, await Retry.RetryAsync(FailsTimes(2), 3));
    [Fact] public async Task Retry_StopsAtFirstSuccess()
    {
        int calls = 0;
        await Retry.RetryAsync(FailsTimes(1, () => calls++), 5);
        Assert.Equal(2, calls);
    }
    [Fact] public async Task Retry_ThrowsLastException()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => Retry.RetryAsync(FailsTimes(9), 3));
        Assert.Equal("fail 3", ex.Message);
    }
    [Fact] public async Task OrDefault_UsesFallback() => Assert.Equal(-1, await Retry.OrDefaultAsync(FailsTimes(1), -1));
    [Fact] public async Task OrDefault_KeepsResult() => Assert.Equal(42, await Retry.OrDefaultAsync(FailsTimes(0), -1));
}
