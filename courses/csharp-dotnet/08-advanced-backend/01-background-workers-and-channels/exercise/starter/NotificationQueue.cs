// Exercise 01 — NotificationQueue with Channel<T>
// See README.md.
using System.Threading.Channels;

public record Notification(string To, string Message);

public class NotificationQueue
{
    private readonly Channel<Notification> _channel = Channel.CreateUnbounded<Notification>();

    public async Task EnqueueAsync(Notification notification)
    {
        throw new NotImplementedException();
    }

    public void CompleteAdding()
    {
        throw new NotImplementedException();
    }

    public async Task<List<Notification>> DrainAllAsync()
    {
        throw new NotImplementedException();
    }
}
