using System.Threading.Channels;

public class Mailbox
{
    private readonly Channel<string> _channel;

    public Mailbox(int capacity)
    {
        throw new NotImplementedException();
    }

    public bool TryPost(string message)
    {
        throw new NotImplementedException();
    }

    public async Task<string> ReadAsync()
    {
        throw new NotImplementedException();
    }
}
