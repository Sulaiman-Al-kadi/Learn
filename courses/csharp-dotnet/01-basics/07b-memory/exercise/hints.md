## Hint 1
Read the count first, then loop:
```csharp
int n = int.Parse(Console.ReadLine() ?? "");
int[] original = new int[n];
for (int i = 0; i < n; i++)
{
    original[i] = int.Parse(Console.ReadLine() ?? "");
}
```

## Hint 2
`int[] alias = original;` copies the address, which is exactly what you want here. For `copy`, use `Clone()` and cast the result: `(int[])original.Clone()`.

## Hint 3
Make `copy` **before** you set `alias[0] = 100`. Otherwise the copy picks up the change.
