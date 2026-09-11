// Capstone — Notifier (solution)
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class NotifierSettings
{
    public int IntervalMinutes { get; set; } = 5;
    public string FromAddress { get; set; } = "";
}

public class Notifier
{
    private readonly NotifierSettings _settings;
    private readonly ILogger<Notifier> _logger;

    public Notifier(IOptions<NotifierSettings> options, ILogger<Notifier> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public void SendNotification(string to, string message)
    {
        if (string.IsNullOrWhiteSpace(_settings.FromAddress))
        {
            _logger.LogWarning("FromAddress is not configured — cannot send");
            return;
        }

        _logger.LogInformation("Sending notification from {From} to {To}: {Message}", _settings.FromAddress, to, message);
    }

    public string Describe()
    {
        return $"Notifier configured to run every {_settings.IntervalMinutes} minutes from {_settings.FromAddress}";
    }
}
