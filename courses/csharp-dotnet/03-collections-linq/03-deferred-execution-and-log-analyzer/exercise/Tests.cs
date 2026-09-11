using Xunit;

public class LogAnalyzerTests
{
    private static List<LogEntry> SampleLogs() => new List<LogEntry>
    {
        new LogEntry(new DateTime(2024, 1, 1, 8, 0, 0), "INFO", "Api", "Started"),
        new LogEntry(new DateTime(2024, 1, 1, 8, 5, 0), "ERROR", "Db", "Connection failed"),
        new LogEntry(new DateTime(2024, 1, 1, 8, 10, 0), "WARN", "Api", "Slow response"),
        new LogEntry(new DateTime(2024, 1, 1, 8, 15, 0), "ERROR", "Api", "critical: null reference"),
        new LogEntry(new DateTime(2024, 1, 1, 8, 20, 0), "ERROR", "Db", "Timeout"),
        new LogEntry(new DateTime(2024, 1, 1, 8, 25, 0), "INFO", "Auth", "User login"),
        new LogEntry(new DateTime(2024, 1, 1, 8, 30, 0), "ERROR", "Auth", "Invalid token"),
    };

    [Fact]
    public void ErrorsOnly_FiltersToErrorLevel()
    {
        var result = LogAnalyzer.ErrorsOnly(SampleLogs());
        Assert.Equal(4, result.Count);
        Assert.All(result, e => Assert.Equal("ERROR", e.Level));
    }

    [Fact]
    public void DistinctSources_FirstOccurrenceOrder()
    {
        var result = LogAnalyzer.DistinctSources(SampleLogs());
        Assert.Equal(new List<string> { "Api", "Db", "Auth" }, result);
    }

    [Fact]
    public void ErrorSummaryBySource_GroupsAndOrdersByCount()
    {
        var result = LogAnalyzer.ErrorSummaryBySource(SampleLogs());
        Assert.Equal(new List<string> { "Db: 2 errors", "Api: 1 errors", "Auth: 1 errors" }, result);
    }

    [Fact]
    public void RecentEntries_ReturnsLatestFirst()
    {
        var result = LogAnalyzer.RecentEntries(SampleLogs(), 3);
        Assert.Equal(new List<string> { "Invalid token", "User login", "Timeout" }, result.Select(e => e.Message).ToList());
    }

    [Fact]
    public void HasCriticalFailure_True_WhenPresent()
    {
        Assert.True(LogAnalyzer.HasCriticalFailure(SampleLogs()));
    }

    [Fact]
    public void HasCriticalFailure_False_WhenAbsent()
    {
        var logsWithoutCritical = SampleLogs().Where(e => !e.Message.Contains("critical")).ToList();
        Assert.False(LogAnalyzer.HasCriticalFailure(logsWithoutCritical));
    }

    [Fact]
    public void PaginatedMessages_PageOne()
    {
        var result = LogAnalyzer.PaginatedMessages(SampleLogs(), 1, 3);
        Assert.Equal(new List<string> { "Started", "Connection failed", "Slow response" }, result);
    }

    [Fact]
    public void PaginatedMessages_PageTwo()
    {
        var result = LogAnalyzer.PaginatedMessages(SampleLogs(), 2, 3);
        Assert.Equal(new List<string> { "critical: null reference", "Timeout", "User login" }, result);
    }
}
