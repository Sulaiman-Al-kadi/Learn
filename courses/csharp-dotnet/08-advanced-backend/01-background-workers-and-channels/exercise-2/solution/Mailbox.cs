using System.Threading.Channels;

public class Mailbox
{
    private readonly Channel<string> _channel;

    public Mailbox(int capacity)
    {
        _channel = Channel.CreateBounded<string>(capacity);
    }

    public bool TryPost(string message)
    {
        return _channel.Writer.TryWrite(message);
    }

    public async Task<string> ReadAsync()
    {
        return await _channel.Reader.ReadAsync();
    }
}
