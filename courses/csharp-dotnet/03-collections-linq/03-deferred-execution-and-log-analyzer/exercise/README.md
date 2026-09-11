# Capstone — Log Analyzer

Write six methods in `LogAnalyzer.cs`, inside a static class `LogAnalyzer`. The `LogEntry` record is already given — don't change it.

```csharp
public record LogEntry(DateTime Timestamp, string Level, string Source, string Message);
```

## Methods (LINQ only)

| Method | Does |
|---|---|
| `List<LogEntry> ErrorsOnly(List<LogEntry> logs)` | keep only entries where `Level == "ERROR"` |
| `List<string> DistinctSources(List<LogEntry> logs)` | every distinct `Source`, first-occurrence order |
| `List<string> ErrorSummaryBySource(List<LogEntry> logs)` | among ERROR entries only: group by `Source`, order by error count descending, format each as `"{Source}: {count} errors"` |
| `List<LogEntry> RecentEntries(List<LogEntry> logs, int count)` | the `count` most recent entries (latest `Timestamp` first) |
| `bool HasCriticalFailure(List<LogEntry> logs)` | is there at least one `ERROR` entry whose `Message` contains `"critical"` (case-insensitive)? |
| `List<string> PaginatedMessages(List<LogEntry> logs, int page, int pageSize)` | entries sorted chronologically (oldest first) by `Timestamp`, page `page` (1-based) of `pageSize` messages, projected to just `Message` |

## Rules
- `ErrorSummaryBySource`: filter to errors first, **then** `GroupBy`, `OrderByDescending` by each group's count, `Select` the formatted string.
- `HasCriticalFailure`: lower-case the message before checking `.Contains("critical")` so it's case-insensitive.
- `PaginatedMessages`: `Skip((page - 1) * pageSize).Take(pageSize)` after sorting — same pattern as the lesson.
- Materialize every result with `.ToList()` at the end of its chain (lesson 03 — don't hand back an un-materialized query).
