# Module 3, Lesson 01 — LINQ Basics

Lesson 08's List exercise built "Unique" and "Second largest" by hand with `foreach` and `Contains`. **LINQ** (Language Integrated Query) does most of that in one line. This is the single skill that separates "I can write a loop" from "I can express what I want directly" — and it's used everywhere in real .NET code, including EF Core database queries (Module 6).

## The `using` you need

```csharp
using System.Linq;
```
(With `ImplicitUsings` enabled, as every project in this course has, this is often already available — but know it's there.)

## `Where` — filtering

```csharp
List<int> numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

List<int> evens = numbers.Where(n => n % 2 == 0).ToList();
// [2, 4, 6, 8, 10]
```

| Piece | Meaning |
|---|---|
| `.Where(...)` | Keep only elements matching a condition. |
| `n => n % 2 == 0` | A **lambda expression** — an inline, unnamed function. Read it as "given `n`, check `n % 2 == 0`." |
| `.ToList()` | LINQ methods return a lazy **query**, not a list (more on this in lesson 03) — `.ToList()` runs it and gives you back an actual `List<T>`. |

Compare to the manual version:

```csharp
List<int> evens = new List<int>();
foreach (int n in numbers)
{
    if (n % 2 == 0) evens.Add(n);
}
```

Same result, one line instead of five. `Where` **is** that loop — it exists so you don't write it by hand every time.

## Lambda expressions — the syntax, precisely

```csharp
n => n % 2 == 0
```
- `n` — the parameter (its type is inferred from context — here, `int`, because `numbers` is `List<int>`).
- `=>` — read as "goes to" or "maps to."
- `n % 2 == 0` — the expression, evaluated and returned.

This is exactly a method (lesson 07), just written inline without a name:

```csharp
bool IsEven(int n) => n % 2 == 0;    // a named method, expression-bodied (lesson 07)
numbers.Where(n => n % 2 == 0);      // the same logic, unnamed, passed directly as an argument
```

Multiple parameters: `(a, b) => a + b`. No parameters: `() => 42`.

## `Select` — transforming (projecting)

```csharp
List<string> names = ["ali", "sara", "omar"];
List<string> capitalized = names.Select(n => n.ToUpper()).ToList();
// ["ALI", "SARA", "OMAR"]

List<int> lengths = names.Select(n => n.Length).ToList();
// [3, 4, 4]
```

`Select` transforms **each** element into something else — same count in, same count out, but possibly a different type entirely (here, `string` → `int`). This is called **projecting**.

## Chaining — the real power

Because `Where` and `Select` both return something you can call more LINQ methods on, you chain them:

```csharp
List<string> names = ["ali", "sara", "omar", "zaid", "lina"];

List<string> result = names
    .Where(n => n.Length > 3)
    .Select(n => n.ToUpper())
    .ToList();
// ["SARA", "OMAR", "ZAID", "LINA"]
```

Read chains top to bottom: "start with `names`, keep only ones longer than 3 characters, uppercase each, then materialize into a list." Each step feeds the next — this reads like a description of *what* you want, not a step-by-step recipe of *how* to loop and accumulate.

## `OrderBy` / `OrderByDescending`

```csharp
List<int> numbers = [5, 2, 8, 1, 9];
List<int> ascending = numbers.OrderBy(n => n).ToList();          // [1, 2, 5, 8, 9]
List<int> descending = numbers.OrderByDescending(n => n).ToList(); // [9, 8, 5, 2, 1]

List<string> names = ["Charlie", "Ali", "Bob"];
List<string> byLength = names.OrderBy(n => n.Length).ToList();    // ["Bob", "Ali", "Charlie"]
```

The lambda given to `OrderBy` is the **sort key** — "order by *this value* computed from each element," not the comparison logic itself. Sorting by string length, by a property, by a computed expression — all the same shape.

## Working with your own types

LINQ works on any `List<T>` (technically any `IEnumerable<T>` — lesson 04's interfaces!), including lists of your own classes/records:

```csharp
public record Student(string Name, int Score);

List<Student> students =
[
    new("Ali", 85),
    new("Sara", 92),
    new("Omar", 67),
];

List<string> passing = students
    .Where(s => s.Score >= 70)
    .Select(s => s.Name)
    .ToList();
// ["Ali", "Sara"]

List<Student> byScore = students.OrderByDescending(s => s.Score).ToList();
```

This is the pattern you'll use constantly: a list of records/objects, filtered and shaped with `Where`/`Select`, in a fraction of the code a manual loop would take.

## Method syntax vs. query syntax (recognize, don't worry about writing)

You'll sometimes see LINQ written like SQL:

```csharp
var evens = from n in numbers
            where n % 2 == 0
            select n;
```

This is **query syntax** — it compiles down to exactly the same `Where`/`Select` calls you just learned (**method syntax**). Method syntax (`.Where(...).Select(...)`) is far more common in real code and is what this course uses throughout. Just recognize query syntax if you see it in older code or tutorials.

## Summary
- `Where(x => condition)` filters; `Select(x => expression)` transforms each element.
- A lambda `x => expr` is an inline, unnamed function — same idea as lesson 07's methods.
- LINQ methods chain: `.Where(...).Select(...).OrderBy(...)`, read top to bottom as a pipeline.
- `.ToList()` / `.ToArray()` materializes the query into a real collection (more on *why* this matters in lesson 03).
- Works on any `List<T>` of your own classes/records, not just built-in types.
