using System.Threading.Channels;
using Xunit;

public class SumWorkerTests
{
    [Fact] public async Task Sum_AllNumbers()
    {
        var channel = Channel.CreateBounded<int>(1);
        Task<int> consumer = SumWorker.RunAsync(channel.Reader);
        await SumWorker.ProduceAsync(channel.Writer, [1, 2, 3, 4]);
        Assert.Equal(10, await consumer.WaitAsync(TimeSpan.FromSeconds(5)));
    }
    [Fact] public async Task Empty_IsZero()
    {
        var channel = Channel.CreateUnbounded<int>();
        await SumWorker.ProduceAsync(channel.Writer, []);
        Assert.Equal(0, await SumWorker.RunAsync(channel.Reader).WaitAsync(TimeSpan.FromSeconds(5)));
    }
    [Fact] public async Task Produce_Completes()
    {
        var channel = Channel.CreateUnbounded<int>();
        await SumWorker.ProduceAsync(channel.Writer, [1]);
        Assert.False(channel.Writer.TryWrite(2));
    }
}
