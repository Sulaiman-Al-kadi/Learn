using System.Threading.Channels;

public static class SumWorker
{
    public static async Task<int> RunAsync(ChannelReader<int> reader)
    {
        int sum = 0;
        await foreach (int n in reader.ReadAllAsync()) sum += n;
        return sum;
    }

    public static async Task ProduceAsync(ChannelWriter<int> writer, IEnumerable<int> numbers)
    {
        foreach (int n in numbers) await writer.WriteAsync(n);
        writer.Complete();
    }
}
