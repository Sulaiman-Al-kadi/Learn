## Hint 1
`ErrorsOnly`: `logs.Where(e => e.Level == "ERROR").ToList();`. `DistinctSources`: `logs.Select(e => e.Source).Distinct().ToList();`

## Hint 2
`ErrorSummaryBySource` chains four steps:
```csharp
return logs
    .Where(e => e.Level == "ERROR")
    .GroupBy(e => e.Source)
    .OrderByDescending(g => g.Count())
    .Select(g => $"{g.Key}: {g.Count()} errors")
    .ToList();
```

## Hint 3
`RecentEntries`: `logs.OrderByDescending(e => e.Timestamp).Take(count).ToList();`

## Hint 4
`HasCriticalFailure`: `logs.Any(e => e.Level == "ERROR" && e.Message.ToLower().Contains("critical"));`

## Hint 5
`PaginatedMessages`: `logs.OrderBy(e => e.Timestamp).Skip((page - 1) * pageSize).Take(pageSize).Select(e => e.Message).ToList();`
