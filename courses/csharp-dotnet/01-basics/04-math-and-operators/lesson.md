# 04 — Math & Operators

An **operator** is a symbol that does something with values: `+` adds, `*` multiplies, `==` compares. This lesson covers the ones you'll use every day, and the two traps that catch every beginner.

## Arithmetic operators

| Operator | Name | Example | Result |
|---|---|---|---|
| `+` | add | `7 + 2` | `9` |
| `-` | subtract | `7 - 2` | `5` |
| `*` | multiply | `7 * 2` | `14` |
| `/` | divide | `7 / 2` | `3` ← **trap #1, see below** |
| `%` | remainder (modulo) | `7 % 2` | `1` |

Normal precedence applies: `*` `/` `%` before `+` `-`. Use parentheses to be explicit:

```csharp
int a = 2 + 3 * 4;      // 14  (3*4 first)
int b = (2 + 3) * 4;    // 20
```

## Trap #1: integer division

When **both** sides of `/` are `int`, the result is an `int` — the decimal part is **thrown away** (not rounded):

```csharp
Console.WriteLine(7 / 2);       // 3   not 3.5
Console.WriteLine(10 / 4);      // 2   not 2.5
Console.WriteLine(1 / 3);       // 0
```

If either side is a `double`, you get real division:

```csharp
Console.WriteLine(7.0 / 2);     // 3.5
Console.WriteLine(7 / 2.0);     // 3.5
double x = 7;
Console.WriteLine(x / 2);       // 3.5
```

Common real-world bug — averaging:

```csharp
int total = 17, count = 4;
double avg = total / count;         // 4  ← WRONG: int/int happened first, then stored as double
double avg2 = (double)total / count; // 4.25 ✓
```

`(double)total` is a **cast**: "treat this value as a double for this expression". Casting one side is enough.

## `%` — the remainder operator

`%` gives what's left after whole-number division. It's more useful than it looks:

```csharp
Console.WriteLine(17 % 5);    // 2  (17 = 3*5 + 2)
Console.WriteLine(10 % 2);    // 0  → 10 is even
Console.WriteLine(11 % 2);    // 1  → 11 is odd
Console.WriteLine(125 % 10);  // 5  → last digit
```

"Is `n` even?" = `n % 2 == 0`. Remember that; it's in every interview.

## Shortcut operators

Updating a variable based on its own value is so common there's shorthand:

```csharp
int score = 10;
score = score + 5;    // the long way
score += 5;           // same thing: "add 5 to score"
score -= 3;           // subtract 3
score *= 2;           // multiply by 2
score /= 4;           // divide by 4
score++;              // add exactly 1
score--;              // subtract exactly 1
```

## Trap #2: `=` vs `==`

- `=` is **assignment**: put a value in a box.
- `==` is **comparison**: are these two equal? Produces a `bool`.

```csharp
int x = 5;            // assignment
bool same = x == 5;   // comparison → true
```

Writing `if (x = 5)` instead of `if (x == 5)` is a classic bug. C# actually catches it (an `int` isn't a `bool`), but in other languages it silently ruins your day.

## Comparison operators — they produce `bool`

| Operator | Meaning |
|---|---|
| `==` | equal |
| `!=` | not equal |
| `<` `>` | less than, greater than |
| `<=` `>=` | less/greater or equal |

```csharp
int age = 20;
Console.WriteLine(age >= 18);     // True
Console.WriteLine(age == 21);     // False
Console.WriteLine("a" == "a");    // True — strings compare by content in C#
```

## Logical operators — combining `bool`s

| Operator | Meaning | `true` when |
|---|---|---|
| `&&` | AND | **both** sides are true |
| `\|\|` | OR | **at least one** side is true |
| `!` | NOT | flips: `!true` is `false` |

```csharp
int age = 20;
bool hasId = true;
Console.WriteLine(age >= 18 && hasId);    // True
Console.WriteLine(age < 18 || !hasId);    // False
```

`&&` and `||` **short-circuit**: if the left side already decides the answer, the right side isn't even evaluated. `false && anything` is `false` without looking at `anything`.

## Working with strings — a few essentials

```csharp
string s = "Hello";
Console.WriteLine(s.Length);          // 5     — number of characters
Console.WriteLine(s.ToUpper());       // HELLO
Console.WriteLine(s.ToLower());       // hello
Console.WriteLine(s + " World");      // Hello World  — + joins strings
Console.WriteLine(s.Contains("ell")); // True
```

`Length` is a **property** (no parentheses — it's a value you read). `ToUpper()` is a **method** (parentheses — an action you run). You'll see this distinction everywhere.

## The `Math` toolbox

```csharp
Console.WriteLine(Math.Max(3, 9));      // 9
Console.WriteLine(Math.Min(3, 9));      // 3
Console.WriteLine(Math.Abs(-4));        // 4
Console.WriteLine(Math.Pow(2, 10));     // 1024  (2 to the power 10) — returns double
Console.WriteLine(Math.Sqrt(16));       // 4
Console.WriteLine(Math.Round(3.14159, 2)); // 3.14
```

## Summary

- `+ - * / %` — and `int / int` **drops the decimal**. Cast to `double` if you need it.
- `%` = remainder. `n % 2 == 0` means even.
- `+=`, `-=`, `++`, `--` are shorthand for updating a variable.
- `=` assigns, `==` compares. Comparisons produce `bool`.
- `&&` and, `||` or, `!` not.
- `s.Length`, `s.ToUpper()`, `Math.Max(...)` etc. are your first tools from the standard library.
