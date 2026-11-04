using Xunit;

public class MailboxTests
{
    [Fact] public void Post_UpToCapacity()
    {
        var box = new Mailbox(2);
        Assert.True(box.TryPost("a"));
        Assert.True(box.TryPost("b"));
        Assert.False(box.TryPost("c"));
    }
    [Fact] public async Task Read_InOrder()
    {
        var box = new Mailbox(2);
        box.TryPost("a");
        box.TryPost("b");
        Assert.Equal("a", await box.ReadAsync());
        Assert.Equal("b", await box.ReadAsync());
    }
    [Fact] public async Task Read_WaitsForPost()
    {
        var box = new Mailbox(1);
        Task<string> reading = box.ReadAsync();
        Assert.False(reading.IsCompleted);
        box.TryPost("late");
        Assert.Equal("late", await reading);
    }
}
