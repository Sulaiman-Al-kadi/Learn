# Exercise — LINQ basics toolkit

Write five methods in `LinqBasics.cs`, inside a static class `LinqTools`. The `Student` record is already given at the top of the file — don't change it.

```csharp
public record Student(string Name, int Score);
```

## Methods (all must use LINQ — no manual `foreach` loops)

| Method | Does |
|---|---|
| `List<int> Evens(List<int> numbers)` | keep only even numbers |
| `List<string> UpperNames(List<string> names)` | uppercase every name |
| `List<string> LongNamesUppercased(List<string> names, int minLength)` | keep names with `Length > minLength`, then uppercase them — **in that order** (filter first, then transform) |
| `List<int> SortedDescending(List<int> numbers)` | sort largest to smallest |
| `List<string> PassingStudentNames(List<Student> students, int passingScore)` | students with `Score >= passingScore`, **ordered by Score descending**, project to just their `Name` |

## Rules
- Every method must be implemented as **one LINQ chain** ending in `.ToList()` — no `foreach`, no manual accumulator lists. That's the point of the exercise.
- `PassingStudentNames` needs three steps chained together: filter (`Where`), sort (`OrderByDescending`), then project (`Select`) — in that order.
