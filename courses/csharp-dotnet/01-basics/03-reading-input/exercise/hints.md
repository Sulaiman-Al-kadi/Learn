## Hint 1
Reading text: 
```csharp
Console.Write("Customer name: ");
string customer = Console.ReadLine() ?? "";
```

## Hint 2
Reading a number is the same, wrapped in a conversion:
```csharp
Console.Write("Price: ");
double price = double.Parse(Console.ReadLine() ?? "");
```
For a whole number use `int.Parse` instead.

## Hint 3
The summary lines are interpolated strings. The last one does math inside the braces: `$"Total: {price * quantity}"`.
