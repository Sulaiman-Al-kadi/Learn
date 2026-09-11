# Module 2, Lesson 09 — Writing Unit Tests

Every exercise since lesson 07 has been checked by tests someone else wrote (`Tests.cs`). Time to flip it: this lesson is about **writing** them yourself. You'll use xUnit, the same tool checking your own work throughout this course.

## Why write tests at all?

A test is a small program that runs your code and checks the result is what you expect — automatically, in milliseconds, every time. Without tests, "does this still work?" means manually re-running the program and eyeballing the output, for every change, forever. With tests, it means pressing one button.

The university plan behind this course requires **≥80% test coverage** on real projects — meaning most of your code is exercised by an automated test. This lesson is the foundation for that.

## Anatomy of an xUnit test

```csharp
using Xunit;

public class CalculatorTests
{
    [Fact]
    public void Add_TwoPositiveNumbers_ReturnsSum()
    {
        int result = Calculator.Add(2, 3);
        Assert.Equal(5, result);
    }
}
```

| Piece | Meaning |
|---|---|
| `[Fact]` | An **attribute** marking this method as a test xUnit should run. Without it, the method is just... a method, never executed. |
| `public void MethodName()` | Tests are `void`, take no parameters, and are `public`. |
| The **name** | `MethodOrFeature_Scenario_ExpectedResult` is a common, readable convention — when this test fails, its name alone tells you what broke. |
| `Assert.Equal(expected, actual)` | The core of a test: state what you expect, then check it. Note the order: **expected first, actual second** — xUnit's failure messages are phrased around that order. |

## The AAA pattern — Arrange, Act, Assert

Nearly every good test has this shape:

```csharp
[Fact]
public void Withdraw_MoreThanBalance_Throws()
{
    // Arrange — set up the situation
    var account = new BankAccount(100);

    // Act — do the one thing you're testing
    void Act() => account.Withdraw(150);

    // Assert — check the outcome
    Assert.Throws<InsufficientFundsException>(Act);
}
```

Keep each test focused on **one** behavior. A test with ten unrelated assertions is hard to read and, when it fails, doesn't tell you clearly which part broke.

## Common `Assert` methods

```csharp
Assert.Equal(5, result);                 // are these equal? (works for value equality — ints, strings, records...)
Assert.NotEqual(0, result);
Assert.True(condition);
Assert.False(condition);
Assert.Null(value);
Assert.NotNull(value);
Assert.Throws<ArgumentException>(() => SomeMethod());     // did calling this throw the expected exception?
Assert.Contains("World", "Hello, World!");
Assert.Empty(list);
Assert.Single(list);                      // exactly one item
Assert.IsAssignableFrom<Shape>(someObject);   // is this actually (at least) this type?
```

`Assert.Throws<T>` takes a **lambda** (`() => ...`) rather than just calling the method directly — if it called the method immediately, the exception would be thrown before `Assert.Throws` ever got a chance to catch and check it. Wrapping it in `() => ...` delays the call until `Assert.Throws` is ready.

## `[Theory]` and `[InlineData]` — one test, many inputs

Testing `IsPrime` for several numbers with separate copy-pasted `[Fact]` methods is repetitive. A `[Theory]` runs the **same** test body once per set of inputs:

```csharp
[Theory]
[InlineData(2, true)]
[InlineData(7, true)]
[InlineData(8, false)]
[InlineData(1, false)]
[InlineData(-3, false)]
public void IsPrime_Works(int number, bool expected)
{
    Assert.Equal(expected, Calculator.IsPrime(number));
}
```

xUnit runs this five times, once per `[InlineData]` row, reporting each as its own pass/fail — exactly the DRY principle (Module 1's methods lesson) applied to tests.

## What makes a good set of tests

- **The normal case** — the obvious, expected input.
- **Edge cases** — the boundary values: `0`, empty string, empty list, the smallest/largest valid input.
- **Invalid input** — what should happen when given something wrong? (Often: it throws — test that with `Assert.Throws`.)
- **One assertion concept per test** — if a test needs a paragraph to explain what it's checking, split it into several tests with clear names.

For `IsPrime`, this means: at least one prime, one non-prime, `1` (not prime, by definition — a very common edge case people forget), `2` (the only even prime), and something ≤ `0`.

## Tests describe behavior, not implementation

Test *what* a method promises to do (its inputs and outputs), not *how* it happens to be written internally. If `IsPrime`'s internal loop changes tomorrow but it still correctly reports whether numbers are prime, none of your tests should need to change. This is what lets you refactor confidently — tests that don't change alongside every internal tweak, yet still catch real breaks, are the ones actually worth having.

## Running tests

```
dotnet test
```
runs every test in a project and reports a summary — exactly what the "Check" button in this course does behind the scenes. You've been running tests since lesson 07 without necessarily thinking of it that way.

## Summary
- `[Fact]` marks a test method; `Assert.Equal(expected, actual)` (and friends) check outcomes.
- **AAA**: Arrange the situation, Act (the one thing under test), Assert the outcome.
- `Assert.Throws<T>(() => ...)` verifies an exception is thrown — wrap the call in a lambda.
- `[Theory]` + `[InlineData(...)]` runs one test body against many input/expected pairs — DRY for tests.
- Good coverage means: the normal case, edge cases, and invalid input — not just the happy path.
- Tests should verify *behavior* (what a method promises), not its internal implementation details.
