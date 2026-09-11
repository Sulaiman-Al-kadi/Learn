# 07 — Methods

Every program you've written so far is one long list of instructions. As programs grow, you need to **name** a piece of logic once and reuse it. That named, reusable piece is a **method**.

## Declaring a method

```csharp
static int Add(int a, int b)
{
    return a + b;
}
```

| Piece | Meaning |
|---|---|
| `static` | For now, always write this. (It means "doesn't belong to a specific object" — you'll learn why in Module 2.) |
| `int` | The **return type** — the type of value this method hands back. |
| `Add` | The method's name. Convention: PascalCase. |
| `(int a, int b)` | The **parameters** — named inputs the caller must provide, each with a type. |
| `{ return a + b; }` | The **body**. `return` hands a value back to the caller and immediately exits the method. |

Calling it:

```csharp
int result = Add(3, 4);          // result is 7
Console.WriteLine(Add(10, 20));  // you can use the call directly, wherever a value is expected
```

`a` and `b` inside the method are **parameters** — placeholders. `3` and `4` at the call site are **arguments** — the actual values. People use the words loosely, but that's the precise distinction.

## Why bother?

Without methods, this logic gets copy-pasted everywhere it's needed:

```csharp
// repeated three times, slightly differently each time — this is how bugs breed
int total1 = price1 * qty1;
int total1WithTax = (int)(total1 * 1.15);
int total2 = price2 * qty2;
int total2WithTax = (int)(total2 * 1.15);
```

With a method, the logic exists **once**:

```csharp
static double TotalWithTax(double price, int qty)
{
    double total = price * qty;
    return total * 1.15;
}

double total1 = TotalWithTax(price1, qty1);
double total2 = TotalWithTax(price2, qty2);
```

Fix a bug in the tax rate once, it's fixed everywhere. This is the **DRY** principle — Don't Repeat Yourself — and it's the single biggest reason methods exist.

## `void` — methods that don't return a value

Some methods just *do* something (print, save, change something) without producing a result:

```csharp
static void Greet(string name)
{
    Console.WriteLine($"Hello, {name}!");
}

Greet("Sara");     // called like a statement, not assigned to anything
```

`void` means "returns nothing." You can still use a bare `return;` (no value) inside to exit early:

```csharp
static void PrintIfPositive(int n)
{
    if (n <= 0) return;             // exit early, do nothing more
    Console.WriteLine(n);
}
```

## Multiple parameters, multiple returns? No — but you can return more

A method returns exactly **one** value. To hand back several related values, return a **tuple**:

```csharp
static (int Min, int Max) MinMax(int a, int b, int c)
{
    int min = Math.Min(a, Math.Min(b, c));
    int max = Math.Max(a, Math.Max(b, c));
    return (min, max);
}

var result = MinMax(5, 2, 9);
Console.WriteLine($"{result.Min} to {result.Max}");

// or unpack directly:
var (lo, hi) = MinMax(5, 2, 9);
Console.WriteLine($"{lo} to {hi}");
```

## Overloading — same name, different parameters

C# lets you define several methods with the same name, as long as their **parameter lists** differ (different count or different types). The compiler picks the right one based on what you pass:

```csharp
static int Square(int n) => n * n;
static double Square(double n) => n * n;

Console.WriteLine(Square(4));      // calls the int version → 16
Console.WriteLine(Square(4.5));    // calls the double version → 20.25
```

(Notice `=>` above — an **expression-bodied method**. When the whole body is one expression, `=> expr;` is a shorter way to write `{ return expr; }`. Common for one-liners.)

## Default parameters and named arguments

```csharp
static string Greet(string name, string greeting = "Hello")
{
    return $"{greeting}, {name}!";
}

Greet("Ali");                       // "Hello, Ali!"          — greeting uses its default
Greet("Ali", "Hi");                 // "Hi, Ali!"
Greet(name: "Ali", greeting: "Hey"); // "Hey, Ali!"            — named arguments, order doesn't matter
```

A parameter with `= value` becomes optional and must come after all required parameters.

## `params` — a variable number of arguments

```csharp
static int Sum(params int[] numbers)
{
    int total = 0;
    foreach (int n in numbers) total += n;
    return total;
}

Sum(1, 2, 3);        // 6
Sum(1, 2, 3, 4, 5);  // 15
Sum();                // 0
```

`params` lets the caller pass any number of values (including zero), and inside the method they arrive as an array.

## Local functions — a method inside a method

Sometimes a helper is only useful inside one method. You can define it right there:

```csharp
static int FactorialSum(int n)
{
    int Factorial(int x) => x <= 1 ? 1 : x * Factorial(x - 1);   // local function, can call itself (recursion!)

    int sum = 0;
    for (int i = 1; i <= n; i++) sum += Factorial(i);
    return sum;
}
```

## Scope — where a variable "lives"

A variable declared inside a method (including its parameters) only exists **inside that method**. This is called **scope**.

```csharp
static void A()
{
    int x = 5;
}

static void B()
{
    Console.WriteLine(x);   // ERROR: x doesn't exist here — it belonged to A's scope only
}
```

This is a *feature*, not a limitation: it means you can reuse simple names like `i`, `total`, `result` in every method without them colliding.

## Summary
- `returnType Name(params) { ... }` — declare once, call by name anywhere.
- `void` = no return value. `return;` exits early with nothing; `return value;` exits with a value.
- Methods are how you avoid repeating logic (DRY).
- Overloading: same name, different parameter lists.
- `= default` makes a parameter optional; `params T[]` accepts any number of arguments.
- A tuple `(int, int)` lets you return more than one value.
- Variables declared inside a method don't exist outside it.
