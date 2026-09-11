# Module 2, Lesson 06 — Generics

You've used `List<int>`, `List<string>`, `Dictionary<string, int>` since Module 1. The `<T>` part is **generics** — writing a class or method **once** that works with any type, decided when it's used. This lesson is about writing your own.

## The problem generics solve

Without generics, to build a simple "box that holds one item" for both `int` and `string`, you'd need two nearly-identical classes:

```csharp
public class IntBox
{
    public int Value;
}

public class StringBox
{
    public string Value = "";
}
```

Copy-pasted logic for every type you need — exactly the repetition Module 1 taught you to avoid with methods.

## A generic class

```csharp
public class Box<T>
{
    public T Value { get; set; }

    public Box(T value)
    {
        Value = value;
    }
}
```

`T` is a **type parameter** — a placeholder. It's filled in with a real type when you use the class:

```csharp
Box<int> intBox = new Box<int>(42);
Box<string> stringBox = new Box<string>("hello");

Console.WriteLine(intBox.Value);      // 42
Console.WriteLine(stringBox.Value);   // hello
```

One class definition, used with any type — and the compiler still enforces type safety: `intBox.Value = "oops";` is a compile error, because `intBox` is specifically a `Box<int>`.

`T` is just a name (by convention, a single capital letter, or `TSomething` for clarity) — it means nothing special; you could call it `TValue`, `TItem`, anything.

## A generic method

Methods can be generic too, independently of whether their class is:

```csharp
public static class Utils
{
    public static T Max<T>(T a, T b) where T : IComparable<T>
    {
        return a.CompareTo(b) > 0 ? a : b;
    }
}

Console.WriteLine(Utils.Max(3, 7));          // 7      — T is int
Console.WriteLine(Utils.Max("ant", "zoo"));  // zoo    — T is string
Console.WriteLine(Utils.Max(2.5, 1.1));      // 2.5    — T is double
```

The compiler infers `T` from the arguments you pass — you don't have to write `Utils.Max<int>(3, 7)` explicitly, though you're allowed to.

## `where T : ...` — constraints

`Max` needs to **compare** two values of type `T`, but not every type supports comparison. `where T : IComparable<T>` is a **constraint**: "only types that implement `IComparable<T>` (lesson 04!) may be used here." This lets the method body call `.CompareTo()`, which the compiler otherwise wouldn't allow on an unconstrained `T` (it has no idea what `T` will be, so by default it assumes only the bare minimum every type has, from `object`).

Common constraints:

```csharp
where T : class            // T must be a reference type
where T : struct           // T must be a value type
where T : IComparable<T>   // T must implement this interface
where T : new()             // T must have a public parameterless constructor (lets you write `new T()`)
where T : BaseClass         // T must be BaseClass or derive from it
```

Without a constraint, all you can do with a `T` is what every type can do (assign it, pass it around, compare with `==` for reference types only if `T` happens to be one) — nothing type-specific.

## A generic class with multiple type parameters

```csharp
public class Pair<TFirst, TSecond>
{
    public TFirst First { get; set; }
    public TSecond Second { get; set; }

    public Pair(TFirst first, TSecond second)
    {
        First = first;
        Second = second;
    }
}

var p = new Pair<string, int>("Sara", 25);
Console.WriteLine($"{p.First} is {p.Second}");   // Sara is 25
```

## Building a small generic data structure — the real payoff

This is where generics genuinely shine: you can implement a reusable structure **once**.

```csharp
public class Stack<T>
{
    private List<T> _items = new List<T>();

    public void Push(T item) => _items.Add(item);

    public T Pop()
    {
        T last = _items[_items.Count - 1];
        _items.RemoveAt(_items.Count - 1);
        return last;
    }

    public T Peek() => _items[_items.Count - 1];

    public int Count => _items.Count;
}
```

```csharp
var numbers = new Stack<int>();
numbers.Push(1);
numbers.Push(2);
Console.WriteLine(numbers.Pop());   // 2 — last in, first out

var names = new Stack<string>();    // the SAME class, completely independently, for a different type
names.Push("Ali");
```

(This is a simplified version of what `System.Collections.Generic.Stack<T>` already gives you — but now you know how it's built, and how `List<T>`, `Dictionary<K,V>` and everything else with angle brackets works under the hood.)

## `default(T)` — the "empty" value for an unknown type

Sometimes you need a placeholder value before you know what `T` will be:

```csharp
public T? Find<T>(List<T> items, Func<T, bool> predicate)
{
    foreach (T item in items)
    {
        if (predicate(item)) return item;
    }
    return default;    // null for reference types, 0/false/etc. for value types
}
```

`default` (or `default(T)`) gives `T`'s "zero value" — `null` for reference types, `0` for `int`, `false` for `bool`, and so on — without needing to know which type `T` actually is.

## Why this matters going forward

Almost everything you'll use in .NET is generic: `List<T>`, `Dictionary<TKey, TValue>`, `Task<T>` (Module 4), `IEnumerable<T>` (Module 3's LINQ), `DbSet<T>` (Module 6's EF Core), `IRepository<T>` (a very common pattern in real backends). Understanding `<T>` here means none of those will feel like magic later.

## Summary
- `class Box<T> { ... }` — one class definition, works with any type, chosen at the call site: `Box<int>`, `Box<string>`.
- Generic methods: `T Max<T>(T a, T b)` — the compiler infers `T` from the arguments.
- `where T : Constraint` restricts what `T` can be, unlocking members that require it (like `IComparable<T>`'s `.CompareTo()`).
- Multiple type parameters: `Pair<TFirst, TSecond>`.
- `default(T)` / `default` gives the type's zero-value without knowing what `T` is.
- Generics avoid duplicating a whole class/method per type — and they're everywhere in .NET.
