# Exercise — Async toolkit

Write three methods in `AsyncTools.cs`, inside a static class `AsyncTools`.

## Methods

```csharp
public static async Task<int> SquareAfterDelayAsync(int n, int delayMs)
```
`await Task.Delay(delayMs)`, then return `n * n`.

```csharp
public static async Task<List<int>> SquareAllAsync(List<int> numbers, int delayMs)
```
Square every number in `numbers`, each with a `delayMs` delay — but run them **concurrently**, not one after another. Return the results in the same order as the input.

```csharp
public static async Task<int> SafeDivideAsync(int a, int b)
```
`await Task.Delay(10)`, then if `b == 0`, `throw new DivideByZeroException()`. Otherwise return `a / b`.

## Rules — this is the important part

For `SquareAllAsync`, **do not** write:
```csharp
// WRONG — this awaits each one before starting the next, so they run SEQUENTIALLY
var results = new List<int>();
foreach (var n in numbers)
{
    results.Add(await SquareAfterDelayAsync(n, delayMs));   // <-- awaiting inside the loop
}
```
Instead: **start** every `SquareAfterDelayAsync` call first (without `await`ing each one individually — that starts it running), collect the `Task<int>` objects, then `await Task.WhenAll(...)` on all of them together. The test measures elapsed time and will fail if your version runs sequentially — three 60ms delays run one-after-another take ~180ms, but running concurrently takes closer to 60ms total.

Hint shape:
```csharp
List<Task<int>> tasks = numbers.Select(n => SquareAfterDelayAsync(n, delayMs)).ToList();   // starts them all
int[] results = await Task.WhenAll(tasks);                                                    // waits for all together
return results.ToList();
```
