# 03 — Reading Input

So far your programs only talk. Now they listen. Reading what the user types is how a program becomes interactive.

## `Console.ReadLine()`

```csharp
Console.Write("What is your name? ");
string name = Console.ReadLine();
Console.WriteLine($"Nice to meet you, {name}!");
```

`Console.ReadLine()` **pauses** the program and waits until the user types something and presses Enter. Whatever they typed is *returned* — handed back — as a `string`, and we store it in `name`.

Notice `Console.Write` (not `WriteLine`) for the question, so the cursor stays on the same line and the user types right after the `?`.

### A wrinkle: it might be `null`

Strictly, `Console.ReadLine()` returns `string?` — the `?` means *"a string, or nothing at all (`null`)"*. That happens if the input is closed with no text. The compiler will show a **warning** (not an error) if you assign it straight to a `string`.

For now, the cleanest fix is to provide a fallback with `??` ("if the left side is null, use the right side instead"):

```csharp
string name = Console.ReadLine() ?? "";
```

Read it as: *"take what was typed, or an empty string if there was nothing."* We'll go much deeper into `null` later; for now just use this pattern.

## Everything typed is text — even numbers

This is the part that trips everyone up:

```csharp
Console.Write("Your age: ");
string ageText = Console.ReadLine() ?? "";
Console.WriteLine(ageText + 1);     // user types 25 → prints 251  (!!)
```

`ReadLine` always gives you a `string`. `"25" + 1` isn't math — it's text joining. To do math you must **convert** the text to a number.

## Converting text to a number — `int.Parse` and `double.Parse`

```csharp
string ageText = Console.ReadLine() ?? "";
int age = int.Parse(ageText);
Console.WriteLine(age + 1);         // user types 25 → prints 26 ✓
```

`int.Parse("25")` reads the characters `2` and `5` and produces the *number* 25. Likewise `double.Parse("3.5")` gives `3.5`.

You can do it in one line:

```csharp
int age = int.Parse(Console.ReadLine() ?? "");
```

### What if they type "hello"?

`int.Parse("hello")` **crashes** the program with a `FormatException` — it can't make a number from that. For now that's acceptable; in a later lesson you'll learn `int.TryParse`, which lets you check first and handle bad input politely.

## A typical interactive program

```csharp
Console.Write("Item name: ");
string item = Console.ReadLine() ?? "";

Console.Write("Price: ");
double price = double.Parse(Console.ReadLine() ?? "");

Console.Write("Quantity: ");
int qty = int.Parse(Console.ReadLine() ?? "");

Console.WriteLine();
Console.WriteLine($"{qty} x {item} = {price * qty}");
```

Running it:

```
Item name: Tea
Price: 4.5
Quantity: 3

3 x Tea = 13.5
```

Note the pattern: **prompt → read → convert (if number) → use**.

## How the checker tests input

The exercise's checker feeds the contents of `input.txt` to your program as if a person typed it — one line per `ReadLine()`. That's why your prompts must match exactly: the checker compares your entire output, prompts included.

Since the "user" doesn't actually type on screen, the typed values will **not** appear in the output — only your prompts and your printed results. So `Console.Write("Name: ")` followed by the printed result looks like `Name: Hello, Sara!` on one line. Keep that in mind when matching the expected output.

## Summary

- `Console.ReadLine()` waits for Enter and returns what was typed as a `string`.
- Use `?? ""` after it to handle the "nothing typed" case without warnings.
- Typed numbers are still text. Convert with `int.Parse(...)` or `double.Parse(...)` before doing math.
- Pattern: prompt with `Write`, read, convert, use.
