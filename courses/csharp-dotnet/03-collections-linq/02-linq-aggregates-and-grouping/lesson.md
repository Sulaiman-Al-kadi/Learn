# Module 3, Lesson 02 — Aggregates, Searching & Grouping

Lesson 01 covered *shaping* a sequence (`Where`, `Select`, `OrderBy`). This lesson covers *reducing* it down to a single answer, *finding* one specific element, and *organizing* it into buckets.

## Aggregates — collapsing a sequence to one value

```csharp
List<int> scores = [85, 92, 67, 78, 90];

Console.WriteLine(scores.Count);              // 5
Console.WriteLine(scores.Sum());               // 412
Console.WriteLine(scores.Average());            // 82.4
Console.WriteLine(scores.Min());                 // 67
Console.WriteLine(scores.Max());                 // 92
```

These replace Module 1's manual accumulator loops (`sum += n` in a `for` loop) with one call.

`Count`, `Min`, and `Max` also accept a condition/selector:

```csharp
Console.WriteLine(scores.Count(s => s >= 80));    // 3   — how many meet a condition
```

```csharp
public record Student(string Name, int Score);
List<Student> students = [new("Ali", 85), new("Sara", 92), new("Omar", 67)];

Console.WriteLine(students.Max(s => s.Score));         // 92 — the max Score, not the max Student
Console.WriteLine(students.Sum(s => s.Score));          // 244 — sum of just the Score values
```

When aggregating **objects**, you nearly always pass a selector lambda — otherwise LINQ has no idea which property to sum/max.

## `Any` and `All` — existence checks

```csharp
List<int> scores = [85, 92, 67];

Console.WriteLine(scores.Any(s => s < 70));       // True  — at least one below 70
Console.WriteLine(scores.All(s => s >= 60));       // True  — every one is at least 60
Console.WriteLine(scores.Any());                    // True  — the list isn't empty (no condition needed)
```

`Any()` with no argument is the idiomatic way to check "is this collection non-empty?" — clearer than `scores.Count > 0` and, for some data sources, faster (it can stop at the first element instead of counting everything).

## Finding one element

```csharp
List<int> scores = [85, 92, 67];

int first = scores.First();                          // 85 — the first element; THROWS if empty
int firstOver90 = scores.First(s => s > 90);           // 92 — first matching; THROWS if none match

int firstOrDefault = scores.FirstOrDefault(s => s > 200);  // 0 (int's default) — no exception
Student? found = students.FirstOrDefault(s => s.Name == "Zara");  // null — no exception for reference types
```

| Method | If none found |
|---|---|
| `First()` / `First(predicate)` | throws `InvalidOperationException` |
| `FirstOrDefault()` / `FirstOrDefault(predicate)` | returns `default` (`0`, `null`, `false`...) — never throws |
| `Single(predicate)` | throws if **zero or more than one** match — use when you expect exactly one |
| `SingleOrDefault(predicate)` | like `Single`, but returns `default` instead of throwing when there are zero matches (still throws if there's more than one) |

**Prefer `FirstOrDefault`/`SingleOrDefault`** over `First`/`Single` whenever "not found" is a normal, expected possibility — same philosophy as `TryParse` over `Parse` (Module 1, lesson 09). Always check the result for `null`/`default` before using it.

## `OrderBy` with multiple keys — `ThenBy`

```csharp
List<Student> students = [new("Ali", 85), new("Sara", 85), new("Omar", 92)];

var sorted = students
    .OrderByDescending(s => s.Score)   // primary sort key
    .ThenBy(s => s.Name)                // tie-breaker, used only when Score is equal
    .ToList();
// Omar(92), Ali(85), Sara(85)   — Ali before Sara because they tie on Score, broken by Name
```

## `GroupBy` — organizing into buckets

```csharp
List<Student> students =
[
    new("Ali", 85), new("Sara", 92), new("Omar", 67), new("Lina", 58), new("Zaid", 74)
];

string Grade(int score) => score switch
{
    >= 90 => "A",
    >= 80 => "B",
    >= 70 => "C",
    _     => "F"
};

var byGrade = students.GroupBy(s => Grade(s.Score));

foreach (var group in byGrade)
{
    Console.WriteLine($"{group.Key}: {string.Join(", ", group.Select(s => s.Name))}");
}
```
```
B: Ali
A: Sara
F: Omar, Lina
C: Zaid
```

`GroupBy` returns a sequence of **groups** — each group has a `.Key` (the value returned by the lambda) and is itself an `IEnumerable<T>` of the elements sharing that key, so you can `foreach`, `.Select()`, `.Count()`, etc. on each group too:

```csharp
var counts = students
    .GroupBy(s => Grade(s.Score))
    .Select(g => $"{g.Key}: {g.Count()} students")
    .ToList();
```

## Putting several together — a realistic pipeline

```csharp
var topThreeNamesByGrade = students
    .Where(s => s.Score >= 70)
    .OrderByDescending(s => s.Score)
    .Take(3)                              // first 3 after sorting — "top 3"
    .Select(s => s.Name)
    .ToList();
```

`Take(n)` keeps the first `n` elements (after whatever ordering came before it); `Skip(n)` discards the first `n` — together they're how pagination ("page 2, 10 per page" = `Skip(10).Take(10)`) is done throughout .NET, including database queries in Module 6.

## Summary
- Aggregates: `Count()`, `Count(predicate)`, `Sum()`, `Average()`, `Min()`, `Max()` — with a selector lambda when working on objects: `.Sum(s => s.Score)`.
- `Any(predicate)` / `All(predicate)` — existence and universality checks. `Any()` alone checks non-empty.
- `First`/`Single` throw when not found; `FirstOrDefault`/`SingleOrDefault` return a safe default instead — prefer these when "not found" is expected.
- `OrderBy(...).ThenBy(...)` — primary sort key, then a tie-breaker.
- `GroupBy(x => key)` buckets elements; each group has `.Key` and is itself enumerable.
- `Skip(n).Take(n)` — pagination.
