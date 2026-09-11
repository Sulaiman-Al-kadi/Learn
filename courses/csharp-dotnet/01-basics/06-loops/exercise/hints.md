## Hint 1
`while (true) { ... if (condition) break; ... }` is a common pattern for "loop until a sentinel value, but I need to read before I can check it."

## Hint 2
Start the running max impossibly low and running min impossibly high, so any real first value replaces them immediately:
```csharp
int max = int.MinValue, min = int.MaxValue;
// each iteration:
max = Math.Max(max, n);
min = Math.Min(min, n);
```

## Hint 3
`{value,4}` inside an interpolated string right-aligns `value` in a field 4 characters wide — that's what makes the table columns line up. The nested loop: outer `for` = rows, inner `for` = columns, `Console.Write` inside, `Console.WriteLine()` after the inner loop ends (to move to the next row).

## Hint 4
Don't forget: if `count == 0`, skip straight from `Numbers: 0` to `Table:` — no Sum/Average/etc lines at all.
