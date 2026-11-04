using Microsoft.Extensions.Logging;
using Xunit;

public class PaymentsTests
{
    private sealed class FakeLogger : ILogger<Payments>
    {
        public readonly List<(LogLevel Level, string Template)> Entries = [];
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel level) => true;
        public void Log<TState>(LogLevel level, EventId id, TState state, Exception? ex, Func<TState, Exception?, string> formatter)
        {
            var values = state as IEnumerable<KeyValuePair<string, object?>>;
            Entries.Add((level, values?.FirstOrDefault(v => v.Key == "{OriginalFormat}").Value?.ToString() ?? ""));
        }
    }

    [Fact] public void Valid_LogsInformationTemplate()
    {
        var log = new FakeLogger();
        Assert.True(new Payments(log).Charge("sara", 5m));
        Assert.Equal((LogLevel.Information, "Charged {User} {Amount}"), log.Entries.Single());
    }
    [Theory] [InlineData(0)] [InlineData(-3)]
    public void Invalid_LogsWarningTemplate(int amount)
    {
        var log = new FakeLogger();
        Assert.False(new Payments(log).Charge("sara", amount));
        Assert.Equal((LogLevel.Warning, "Rejected charge for {User}"), log.Entries.Single());
    }
}
