# Module 3, Lesson 03 — Deferred Execution & the Log Analyzer Capstone

One more essential LINQ idea, then a capstone project combining everything from this module.

## LINQ queries are lazy — they don't run until you ask for the results

```csharp
List<int> numbers = [1, 2, 3];

var query = numbers.Where(n =>
{
    Console.WriteLine($"checking {n}");
    return n > 1;
});

Console.WriteLine("query created, but nothing printed yet above this line");

foreach (int n in query)         // THIS is when Where's lambda actually runs, once per element
{
    Console.WriteLine($"got {n}");
}
```

Output:
```
query created, but nothing printed yet above this line
checking 1
checking 2
got 2
checking 3
got 3
```

`numbers.Where(...)` doesn't loop through anything immediately — it builds a **description** of the query. The lambda only actually runs when something **enumerates** the result: a `foreach`, or a call like `.ToList()`, `.Count()`, `.First()`. This is called **deferred execution**.

## Why this matters: the multiple-enumeration trap

```csharp
var evens = numbers.Where(n => n % 2 == 0);   // not a List<int> — a QUERY

int count = evens.Count();     // runs the query once
var list = evens.ToList();      // runs the query AGAIN, from scratch
```

If the source changes between those two lines (or if the `Where` lambda is doing something expensive, or has a side effect like the `Console.WriteLine` above), you get surprising, inconsistent, or slow behavior — the query re-executes every single time you enumerate it, not once and cached.

**The fix:** call `.ToList()` (or `.ToArray()`) **once**, as soon as you're done building the query, and work with that materialized list from then on:

```csharp
List<int> evens = numbers.Where(n => n % 2 == 0).ToList();   // runs ONCE, now it's a real list

int count = evens.Count;    // just reads .Count — no re-running anything
var copy = evens.ToList();   // just copies a list — no re-running the Where lambda
```

This is exactly the same caution from Module 1's memory lesson about `IEnumerable<T>` — LINQ is where it comes up constantly in practice.

## `Distinct()` — removing duplicates

```csharp
List<string> tags = ["error", "warn", "error", "info", "warn"];
List<string> unique = tags.Distinct().ToList();
// ["error", "warn", "info"]  — first occurrence of each kept, order preserved
```

This replaces lesson 08's manual "walk the list, `Contains` check, `Add` if new" pattern with one call.

## `Skip` / `Take` — pagination, revisited

```csharp
var page2 = items.OrderBy(i => i.Id).Skip(10).Take(10).ToList();   // items 11–20
```

## Putting the whole module together

A realistic LINQ pipeline reads top to bottom like a sentence: filter, sort, group, shape, materialize.

```csharp
var report = logEntries
    .Where(e => e.Level == "ERROR")              // lesson 01
    .GroupBy(e => e.Source)                        // lesson 02
    .OrderByDescending(g => g.Count())              // lesson 02
    .Select(g => $"{g.Key}: {g.Count()} errors")    // lesson 01
    .ToList();                                        // this lesson — materialize once, at the end
```

## Summary
- LINQ queries are **lazy** — nothing runs until enumerated (`foreach`, `.ToList()`, `.Count()`, etc.).
- Each enumeration re-runs the whole query — call `.ToList()` **once** and reuse the result, rather than enumerating the same query object repeatedly.
- `Distinct()` removes duplicates, preserving first-occurrence order.
- `Skip(n).Take(n)` — pagination.
- A LINQ pipeline reads as a sequence of steps: filter → sort/group → shape → materialize.

## Capstone: Log Analyzer

Open the exercise. You're given a list of parsed log entries and asked to answer several realistic questions about them — exactly the kind of thing LINQ was built for, and exactly the kind of code you'll write constantly once you reach Module 6 (EF Core) and Module 8 (structured logging with Serilog).
