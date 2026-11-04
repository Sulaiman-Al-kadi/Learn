using Microsoft.Extensions.Options;

public class MailSettings
{
    public string Host { get; set; } = "";
    public int Port { get; set; }
    public string From { get; set; } = "";
}

public class Mailer
{
    private readonly MailSettings _settings;

    public Mailer(IOptions<MailSettings> options)
    {
        _settings = options.Value;
    }

    public string From()
    {
        return _settings.From;
    }

    public bool IsConfigured()
    {
        return !string.IsNullOrWhiteSpace(_settings.Host) && _settings.Port > 0;
    }

    public string Describe()
    {
        return IsConfigured() ? $"{_settings.Host}:{_settings.Port}" : "not configured";
    }
}
