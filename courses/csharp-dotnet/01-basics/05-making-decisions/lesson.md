# 05 — Making Decisions (`if` / `else`)

Until now every program ran the same lines top to bottom, every time. Real programs **choose** what to do based on conditions: *if the password is right, log in; otherwise show an error.*

## `if`

```csharp
int temperature = 35;

if (temperature > 30)
{
    Console.WriteLine("It's hot!");
}
```

| Piece | Meaning |
|---|---|
| `if` | keyword: "only do the following when..." |
| `(temperature > 30)` | the **condition** — any expression that produces a `bool` (lesson 04!) |
| `{ ... }` | the **block** — the statements that run only if the condition is `true` |

If the condition is `false`, the whole block is skipped and the program continues after the closing `}`.

Note: **no semicolon** after `if (...)` or after `}`. A semicolon there is a subtle bug — `if (x > 3);` means "if x > 3, do nothing" and then the block always runs.

## `else` — otherwise

```csharp
if (temperature > 30)
{
    Console.WriteLine("It's hot!");
}
else
{
    Console.WriteLine("It's fine.");
}
```

Exactly one of the two blocks runs. Never both, never neither.

## `else if` — several options

```csharp
int score = 85;

if (score >= 90)
{
    Console.WriteLine("A");
}
else if (score >= 80)
{
    Console.WriteLine("B");
}
else if (score >= 70)
{
    Console.WriteLine("C");
}
else
{
    Console.WriteLine("F");
}
```

Conditions are checked **top to bottom**, and the **first** true one wins — the rest are skipped. With `score = 85`: `85 >= 90`? no. `85 >= 80`? yes → prints `B`, done. It never checks `>= 70` even though that's also true.

That's why **order matters**. If you wrote `>= 70` first, a 95 would get a C.

## Conditions can be combined

Everything from lesson 04 works here:

```csharp
int age = 20;
bool hasLicense = true;

if (age >= 18 && hasLicense)
{
    Console.WriteLine("You may drive.");
}
else if (age >= 18 && !hasLicense)
{
    Console.WriteLine("Get a license first.");
}
else
{
    Console.WriteLine("Too young.");
}
```

## Comparing strings

```csharp
string answer = Console.ReadLine() ?? "";

if (answer == "yes")
{
    Console.WriteLine("Great!");
}
```

Careful: `"Yes"` and `"yes"` are different. To ignore case, normalize first: `if (answer.ToLower() == "yes")`.

## Nested `if` — an `if` inside an `if`

```csharp
if (isLoggedIn)
{
    if (isAdmin)
    {
        Console.WriteLine("Admin panel");
    }
    else
    {
        Console.WriteLine("User dashboard");
    }
}
else
{
    Console.WriteLine("Please log in");
}
```

Nesting works, but more than two levels deep gets hard to read. Often you can flatten with `&&`: `if (isLoggedIn && isAdmin)`.

## Braces: always use them

C# lets you skip `{ }` when the block is a single statement:

```csharp
if (x > 5)
    Console.WriteLine("big");   // legal
```

Don't. The moment you add a second line, it silently runs *outside* the `if`:

```csharp
if (x > 5)
    Console.WriteLine("big");
    Console.WriteLine("really big");   // ALWAYS runs — indentation means nothing to C#
```

Always write the braces. Every professional codebase requires it.

## The conditional operator `? :` — a one-line if/else for values

When you just need to *pick a value*, there's a compact form:

```csharp
string label = score >= 50 ? "pass" : "fail";
//             condition   ? if-true : if-false
```

Read it as: *"score at least 50? then 'pass', otherwise 'fail'."* Great for short choices inside interpolation: `$"You {(passed ? "passed" : "failed")}"`. Don't chain more than one — use `else if` instead.

## Summary

- `if (condition) { ... }` runs the block only when the condition is `true`.
- `else { ... }` runs when it's `false`. `else if` chains more options; **first match wins, order matters**.
- No `;` after `if (...)`. Always use `{ }`.
- Conditions are `bool` expressions — combine with `&&`, `||`, `!`.
- `condition ? a : b` picks one of two values.
