// Capstone — Notifier
// See README.md.
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class NotifierSettings
{
    public int IntervalMinutes { get; set; } = 5;
    public string FromAddress { get; set; } = "";
}

public class Notifier
{
    // TODO: private readonly NotifierSettings field (store options.Value), private readonly ILogger<Notifier> field
    public Notifier(IOptions<NotifierSettings> options, ILogger<Notifier> logger)
    {
        throw new NotImplementedException();
    }

    public void SendNotification(string to, string message)
    {
        throw new NotImplementedException();
    }

    public string Describe()
    {
        throw new NotImplementedException();
    }
}
