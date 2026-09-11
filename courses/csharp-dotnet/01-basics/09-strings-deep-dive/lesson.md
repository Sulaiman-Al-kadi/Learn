# 09 — Strings Deep Dive

You've used strings since lesson 01. Now the real toolbox — the methods you'll reach for constantly when processing text (and you'll process a lot of text: user input, files, API responses).

## Strings are immutable — a reminder with consequences

Every string method **returns a new string**; it never changes the original:

```csharp
string s = "hello";
s.ToUpper();                    // does NOTHING to s — the result is thrown away!
Console.WriteLine(s);           // hello

string upper = s.ToUpper();     // correct: capture the returned value
Console.WriteLine(upper);       // HELLO
```

Forgetting to use the return value is a very common mistake with strings specifically, because with lists (`.Add()`, `.Sort()`) the *object itself* changes. Strings never do — every operation makes a new one.

## Inspecting a string

```csharp
string s = "Hello, World!";
Console.WriteLine(s.Length);              // 13
Console.WriteLine(s[0]);                  // 'H'   — indexing gives a single char
Console.WriteLine(s.Contains("World"));   // True
Console.WriteLine(s.StartsWith("Hello")); // True
Console.WriteLine(s.EndsWith("!"));       // True
Console.WriteLine(s.IndexOf("World"));    // 7     — position where it starts, -1 if not found
Console.WriteLine(s.IsNullOrEmpty(s));    // this is wrong — see below
```

`IsNullOrEmpty` and `IsNullOrWhiteSpace` are **static** methods on `string` itself — you call them as `string.IsNullOrEmpty(s)`, not `s.IsNullOrEmpty()`, because if `s` is `null` there's no object to call the method *on*:

```csharp
string? input = Console.ReadLine();
if (string.IsNullOrWhiteSpace(input))     // true for null, "", or "   "
{
    Console.WriteLine("You typed nothing useful.");
}
```

## Transforming a string

```csharp
string s = "  Hello, World!  ";
Console.WriteLine(s.Trim());              // "Hello, World!"     — strips leading/trailing whitespace
Console.WriteLine(s.ToUpper());           // "  HELLO, WORLD!  "
Console.WriteLine(s.ToLower());           // "  hello, world!  "
Console.WriteLine(s.Replace("World", "C#")); // "  Hello, C#!  "
Console.WriteLine(s.Trim().Replace(",", ""));  // methods CHAIN — each returns a string you can call another method on
```

Chaining is idiomatic: `s.Trim().ToLower()` reads left to right as "trim it, then lowercase the result."

## Slicing — `Substring`

```csharp
string s = "Hello, World!";
Console.WriteLine(s.Substring(7));       // "World!"      — from index 7 to the end
Console.WriteLine(s.Substring(7, 5));    // "World"       — from index 7, take 5 characters
Console.WriteLine(s.Substring(0, 5));    // "Hello"
```

`Substring(start, length)` — the second number is a **count**, not an end index. Mixing that up is a common bug.

## Splitting and joining

These two are the workhorses of parsing text — you'll use them constantly.

```csharp
string csv = "Ali,25,Riyadh";
string[] parts = csv.Split(',');
// parts = ["Ali", "25", "Riyadh"]

string name = parts[0];
int age = int.Parse(parts[1]);

string rejoined = string.Join(" - ", parts);
// "Ali - 25 - Riyadh"
```

`Split` also takes multiple separators and can remove empty entries — handy for messy input:

```csharp
string messy = "a,, b,,c";
string[] clean = messy.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
// ["a", "b", "c"]
```

## Checking and converting case-insensitively

```csharp
string a = "YES", b = "yes";
Console.WriteLine(a == b);                                   // False — exact match required
Console.WriteLine(a.ToLower() == b.ToLower());                // True
Console.WriteLine(string.Equals(a, b, StringComparison.OrdinalIgnoreCase)); // True — the "proper" way
```

`string.Equals(..., StringComparison.OrdinalIgnoreCase)` is preferred over `.ToLower() ==` in real code (it doesn't allocate a new string and handles some edge cases better), but `.ToLower() ==` is perfectly fine and more readable while you're learning.

## Building a string efficiently — `StringBuilder`

You met this in Module 1's memory lesson: `+=` in a loop allocates a new string every time. For anything beyond a handful of concatenations, use `StringBuilder`:

```csharp
using System.Text;

var sb = new StringBuilder();
for (int i = 1; i <= 5; i++)
{
    sb.Append(i);
    sb.Append(", ");
}
string result = sb.ToString();     // "1, 2, 3, 4, 5, "
```

`Append` mutates the `StringBuilder` in place (unlike `string`, it's **not** immutable) — that's the whole point, no new object every call.

## Parsing numbers safely — `TryParse`

Lesson 03 used `int.Parse`, which **crashes** on bad input. `TryParse` instead **tells you** whether it worked, via a `bool`, and never throws:

```csharp
string input = "abc";
bool ok = int.TryParse(input, out int result);

if (ok)
{
    Console.WriteLine($"Parsed: {result}");
}
else
{
    Console.WriteLine("That wasn't a number.");
}
```

`out int result` is a new kind of parameter: the method fills it in as a *side effect*, in addition to returning `bool`. You can also write the check and use inline:

```csharp
if (int.TryParse(Console.ReadLine(), out int age) && age > 0)
{
    Console.WriteLine($"Valid age: {age}");
}
```

**From now on, prefer `TryParse` over `Parse`** whenever input might be invalid — which is almost always, for anything a user typed.

## Summary
- Strings are immutable — every method returns a **new** string; always capture it.
- `Trim`, `ToUpper`/`ToLower`, `Replace`, `Contains`, `StartsWith`/`EndsWith`, `IndexOf` — inspect and transform.
- `Substring(start, length)` — length is a count, not an end index.
- `Split` / `string.Join` — parse and rebuild delimited text.
- `string.IsNullOrEmpty(s)` / `string.Equals(a, b, StringComparison.OrdinalIgnoreCase)` — static methods, safe with `null`.
- `StringBuilder` for building strings in a loop.
- `int.TryParse(text, out int n)` — safe parsing that never crashes; prefer it over `Parse`.
