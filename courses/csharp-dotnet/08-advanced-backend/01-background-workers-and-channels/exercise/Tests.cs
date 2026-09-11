using Xunit;

public class NotificationQueueTests
{
    [Fact]
    public async Task EnqueueThenDrain_ReturnsItemsInOrder()
    {
        var queue = new NotificationQueue();
        await queue.EnqueueAsync(new Notification("ali@example.com", "Welcome!"));
        await queue.EnqueueAsync(new Notification("sara@example.com", "Reminder"));
        queue.CompleteAdding();

        var drained = await queue.DrainAllAsync();

        Assert.Equal(2, drained.Count);
        Assert.Equal("ali@example.com", drained[0].To);
        Assert.Equal("sara@example.com", drained[1].To);
    }

    [Fact]
    public async Task Drain_EmptyQueue_ReturnsEmptyList()
    {
        var queue = new NotificationQueue();
        queue.CompleteAdding();

        var drained = await queue.DrainAllAsync();

        Assert.Empty(drained);
    }

    [Fact]
    public async Task MultipleProducers_AllItemsAreEnqueued()
    {
        var queue = new NotificationQueue();

        var producers = Enumerable.Range(1, 5)
            .Select(i => queue.EnqueueAsync(new Notification($"user{i}@example.com", $"Message {i}")));
        await Task.WhenAll(producers);
        queue.CompleteAdding();

        var drained = await queue.DrainAllAsync();

        Assert.Equal(5, drained.Count);
        Assert.Contains(drained, n => n.To == "user3@example.com");
    }
}
