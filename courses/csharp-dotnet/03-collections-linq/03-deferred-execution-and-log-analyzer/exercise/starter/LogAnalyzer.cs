// Capstone — Log Analyzer
// See README.md. LINQ only — every method should end in .ToList() (or return a bool directly).

public record LogEntry(DateTime Timestamp, string Level, string Source, string Message);

public static class LogAnalyzer
{
    public static List<LogEntry> ErrorsOnly(List<LogEntry> logs)
    {
        throw new NotImplementedException();
    }

    public static List<string> DistinctSources(List<LogEntry> logs)
    {
        throw new NotImplementedException();
    }

    public static List<string> ErrorSummaryBySource(List<LogEntry> logs)
    {
        throw new NotImplementedException();
    }

    public static List<LogEntry> RecentEntries(List<LogEntry> logs, int count)
    {
        throw new NotImplementedException();
    }

    public static bool HasCriticalFailure(List<LogEntry> logs)
    {
        throw new NotImplementedException();
    }

    public static List<string> PaginatedMessages(List<LogEntry> logs, int page, int pageSize)
    {
        throw new NotImplementedException();
    }
}
