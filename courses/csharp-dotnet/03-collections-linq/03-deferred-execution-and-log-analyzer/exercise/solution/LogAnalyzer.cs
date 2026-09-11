// Capstone — Log Analyzer (solution)

public record LogEntry(DateTime Timestamp, string Level, string Source, string Message);

public static class LogAnalyzer
{
    public static List<LogEntry> ErrorsOnly(List<LogEntry> logs)
    {
        return logs.Where(e => e.Level == "ERROR").ToList();
    }

    public static List<string> DistinctSources(List<LogEntry> logs)
    {
        return logs.Select(e => e.Source).Distinct().ToList();
    }

    public static List<string> ErrorSummaryBySource(List<LogEntry> logs)
    {
        return logs
            .Where(e => e.Level == "ERROR")
            .GroupBy(e => e.Source)
            .OrderByDescending(g => g.Count())
            .Select(g => $"{g.Key}: {g.Count()} errors")
            .ToList();
    }

    public static List<LogEntry> RecentEntries(List<LogEntry> logs, int count)
    {
        return logs.OrderByDescending(e => e.Timestamp).Take(count).ToList();
    }

    public static bool HasCriticalFailure(List<LogEntry> logs)
    {
        return logs.Any(e => e.Level == "ERROR" && e.Message.ToLower().Contains("critical"));
    }

    public static List<string> PaginatedMessages(List<LogEntry> logs, int page, int pageSize)
    {
        return logs
            .OrderBy(e => e.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => e.Message)
            .ToList();
    }
}
