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
        throw new NotImplementedException();
    }

    public string From()
    {
        throw new NotImplementedException();
    }

    public bool IsConfigured()
    {
        throw new NotImplementedException();
    }

    public string Describe()
    {
        throw new NotImplementedException();
    }
}
