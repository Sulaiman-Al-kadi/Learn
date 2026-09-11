# Exercise — Write tests for Calculator

This exercise is reversed from every other one: `Calculator.cs` is already written and fixed (open it — you don't edit it). **You** write `CalculatorTests.cs`.

## What to test

Write `[Fact]` (or `[Theory]`) tests in `CalculatorTests.cs` covering:

1. `Add` — a normal case, e.g. `Add(2, 3)` is `5`.
2. `Subtract` — a normal case.
3. `Divide` — a normal case, e.g. `Divide(10, 2)` is `5`.
4. `Divide` by zero — throws `DivideByZeroException`. Use `Assert.Throws<DivideByZeroException>(() => ...)`.
5. `IsPrime` — at least these cases, each is worth its own test or `[InlineData]` row:
   - a prime number (e.g. `7`) → `true`
   - a non-prime number (e.g. `8`) → `false`
   - `1` → `false` (**not** prime, by definition — the classic edge case)
   - `2` → `true` (the only even prime)
   - `0` or a negative number → `false`

That's a minimum of **8 passing test cases** across your `[Fact]`/`[Theory]` methods.

## Starting point

The starter file has **one test already written — and it's wrong on purpose**. Fix its expected value first (that's your first passing test), then add the rest following the same pattern.

## Rules
- Every test method needs `[Fact]` (or `[Theory]` + `[InlineData]`).
- Use `Assert.Equal`, `Assert.True`/`Assert.False`, and `Assert.Throws` as covered in the lesson.
- Give each test a clear name: `MethodName_Scenario_ExpectedResult`.
- The checker just runs `dotnet test` — if you write zero tests, or tests that don't actually check anything meaningful, you're only cheating your own practice. Write real ones.
