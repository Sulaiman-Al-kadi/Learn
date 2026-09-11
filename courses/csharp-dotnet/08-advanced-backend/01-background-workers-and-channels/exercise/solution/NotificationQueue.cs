// Exercise 01 — NotificationQueue with Channel<T> (solution)
using System.Threading.Channels;

public record Notification(string To, string Message);

public class NotificationQueue
{
    private readonly Channel<Notification> _channel = Channel.CreateUnbounded<Notification>();

    public async Task EnqueueAsync(Notification notification)
    {
        await _channel.Writer.WriteAsync(notification);
    }

    public void CompleteAdding()
    {
        _channel.Writer.Complete();
    }

    public async Task<List<Notification>> DrainAllAsync()
    {
        var results = new List<Notification>();
        await foreach (Notification notification in _channel.Reader.ReadAllAsync())
        {
            results.Add(notification);
        }
        return results;
    }
}
