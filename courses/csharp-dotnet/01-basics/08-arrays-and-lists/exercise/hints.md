## Hint 1
Reading n values: `for (int i = 0; i < n; i++) { original.Add(int.Parse(Console.ReadLine() ?? "")); }`

## Hint 2
Copy-then-mutate is the key idea for Sorted/Reversed — copying with `new List<int>(original)` means `.Sort()`/`.Reverse()` don't disturb `original`, which you still need later.

## Hint 3
Unique:
```csharp
List<int> unique = new List<int>();
foreach (int value in original)
{
    if (!unique.Contains(value)) unique.Add(value);
}
```

## Hint 4
Second largest, one pass over `unique`: keep `largest` and `second`, both starting at `int.MinValue`. For each value: if it beats `largest`, the old `largest` becomes the new `second` (as long as `largest` wasn't still the starting sentinel), and the value becomes the new `largest`. Otherwise, if it beats `second`, it becomes the new `second`. At the end, `unique.Count >= 2` tells you whether a real second value was found.
