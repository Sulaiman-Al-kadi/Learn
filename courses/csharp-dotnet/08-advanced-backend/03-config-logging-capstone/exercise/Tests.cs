using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

// A minimal fake ILogger<T> that records every logged message so tests can assert on
// what was logged, at what level — a common technique for testing structured logging
// without needing a real logging provider (Serilog, etc.) wired up.
public class FakeLogger<T> : ILogger<T>
{
    public record Entry(LogLevel Level, string Message);
    public List<Entry> Entries { get; } = new List<Entry>();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Entries.Add(new Entry(logLevel, formatter(state, exception)));
    }
}

public class NotifierTests
{
    private static (Notifier Notifier, FakeLogger<Notifier> Logger) Make(string fromAddress, int intervalMinutes = 5)
    {
        var settings = Options.Create(new NotifierSettings { FromAddress = fromAddress, IntervalMinutes = intervalMinutes });
        var logger = new FakeLogger<Notifier>();
        return (new Notifier(settings, logger), logger);
    }

    [Fact]
    public void SendNotification_ValidFromAddress_LogsInformationWithStructuredFields()
    {
        var (notifier, logger) = Make("noreply@example.com");

        notifier.SendNotification("ali@example.com", "Reminder");

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, entry.Level);
        Assert.Contains("noreply@example.com", entry.Message);
        Assert.Contains("ali@example.com", entry.Message);
        Assert.Contains("Reminder", entry.Message);
    }

    [Fact]
    public void SendNotification_MissingFromAddress_LogsWarningOnly()
    {
        var (notifier, logger) = Make("");

        notifier.SendNotification("ali@example.com", "Reminder");

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
    }

    [Fact]
    public void Describe_FormatsSettingsCorrectly()
    {
        var (notifier, _) = Make("noreply@example.com", 15);

        string description = notifier.Describe();

        Assert.Equal("Notifier configured to run every 15 minutes from noreply@example.com", description);
    }
}
