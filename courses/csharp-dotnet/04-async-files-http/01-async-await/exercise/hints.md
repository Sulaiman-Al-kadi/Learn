## Hint 1
`SquareAfterDelayAsync`: `await Task.Delay(delayMs); return n * n;`

## Hint 2
`SquareAllAsync` — start all the tasks BEFORE awaiting any of them:
```csharp
List<Task<int>> tasks = numbers.Select(n => SquareAfterDelayAsync(n, delayMs)).ToList();
int[] results = await Task.WhenAll(tasks);
return results.ToList();
```
Calling `SquareAfterDelayAsync(n, delayMs)` without `await` starts it running immediately and gives you back the `Task<int>` — collecting several of those THEN awaiting them together with `Task.WhenAll` is what makes them run concurrently.

## Hint 3
`SafeDivideAsync`: `await Task.Delay(10); if (b == 0) throw new DivideByZeroException(); return a / b;`
