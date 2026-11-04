using System.Threading.Channels;

public static class SumWorker
{
    public static async Task<int> RunAsync(ChannelReader<int> reader)
    {
        throw new NotImplementedException();
    }

    public static async Task ProduceAsync(ChannelWriter<int> writer, IEnumerable<int> numbers)
    {
        throw new NotImplementedException();
    }
}
