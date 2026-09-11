# Module 2, Lesson 05 — Records & Value Equality

You've written `class` for everything so far. C# has a second kind of type, `record`, built for a very common case: **data that represents a value**, not an object with identity and behavior.

## The problem: classes compare by reference

```csharp
public class Point
{
    public int X { get; set; }
    public int Y { get; set; }
}

var p1 = new Point { X = 1, Y = 2 };
var p2 = new Point { X = 1, Y = 2 };
Console.WriteLine(p1 == p2);          // False!
Console.WriteLine(p1.Equals(p2));     // False!
```

Both points hold identical data, yet C# says they're not equal — because for a `class`, `==` and `.Equals()` default to asking "are these literally the same object in memory?" (Module 1's memory lesson: reference equality.) For coordinates, prices, dates — pure **values** — that's almost never what you want. Two `(1, 2)` points *should* be considered equal.

## `record` — value equality, for free

```csharp
public record Point(int X, int Y);
```

This one line declares a type with `X` and `Y` **properties**, a constructor that sets them, and — the key difference — `==`/`.Equals()` that compare **field by field**:

```csharp
var p1 = new Point(1, 2);
var p2 = new Point(1, 2);
Console.WriteLine(p1 == p2);       // True — same values
Console.WriteLine(p1.Equals(p2));  // True
Console.WriteLine(ReferenceEquals(p1, p2));  // False — still two separate objects on the heap!
```

`p1` and `p2` are still two distinct objects (just like before) — but the record's generated `Equals`/`==` look at the **data**, not the memory address. This is called **value equality** (or structural equality).

## `record` also gives you a useful `ToString()`

```csharp
Console.WriteLine(p1);
// Point { X = 1, Y = 2 }
```

Compare to a plain `class`, where `Console.WriteLine(obj)` just prints the type name (`Point`) unless you write your own `ToString()` override. Records generate a readable one automatically.

## This positional syntax is shorthand for properties

`public record Point(int X, int Y);` is equivalent to writing:

```csharp
public record Point
{
    public int X { get; init; }
    public int Y { get; init; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
    // + auto-generated Equals, GetHashCode, ToString
}
```

Notice `init`, not `set` (lesson 02) — by default, record properties declared this way are **immutable after construction**. `p1.X = 5;` is a compile error. This is deliberate: records model values, and values that can silently change from under you defeat the purpose (imagine a `Money` amount changing while something else still holds a reference to it, expecting it to be stable).

## `with` — creating a modified copy

Since you can't mutate a record's properties directly, how do you "change" one? You don't — you create a **new** record, copying every property except the ones you specify:

```csharp
public record Point(int X, int Y);

var original = new Point(1, 2);
var moved = original with { X = 10 };     // new object: X = 10, Y = 2 (copied)

Console.WriteLine(original);   // Point { X = 1, Y = 2 }   ← unchanged
Console.WriteLine(moved);      // Point { X = 10, Y = 2 }
Console.WriteLine(original == moved);   // False — different X values now
```

`with` is non-destructive: the original is never touched. This pattern — never mutate, always produce a new value — is called **immutability**, and it eliminates a whole category of bugs where some other piece of code held onto an object and got surprised when it changed underneath it.

## Records can still have methods, and more properties than the constructor

```csharp
public record Money(decimal Amount, string Currency)
{
    public string Formatted => $"{Amount:0.00} {Currency}";     // computed property, same as lesson 02

    public Money Add(Money other)
    {
        if (other.Currency != Currency) throw new ArgumentException("Currency mismatch");
        return this with { Amount = Amount + other.Currency.Length * 0 + other.Amount };
    }
}
```

Records are still classes under the hood (there's also `record struct` for a value-type version) — you can add methods, computed properties, even inheritance between records. The positional parameters just save you from writing the boilerplate for the common case.

## When to use `record` vs `class`

| | `record` | `class` |
|---|---|---|
| Equality | by value (data) | by reference (identity), unless you override it |
| Typical use | DTOs, API responses, coordinates, money, events — "this data" | Things with behavior, mutable state, identity — services, `BankAccount`, `Car` |
| Mutability | immutable by default (`init`) | mutable by default (`set`) |
| "Changing" one | `with` → new copy | mutate in place |

**Rule of thumb:** if two instances with the same data should be considered "the same," and nothing about the type needs to change after creation, reach for `record`. If it represents something with an ongoing identity and changing state (a `Car`'s mileage increasing, a `BankAccount`'s balance), use `class`.

## `record struct` — briefly

```csharp
public record struct Point(int X, int Y);
```
Combines record's value equality with `struct`'s value-type storage (Module 1's memory lesson: copied on assignment, no heap allocation). Use for small, very frequently-created values (like a 2D point used millions of times in a game loop) where allocation matters. For everyday use, plain `record` is fine.

## Summary
- `record Name(Type Prop1, Type Prop2);` — one line gives you properties, a constructor, value-based `Equals`/`==`, and a readable `ToString()`.
- Properties from the positional syntax are `init`-only — immutable after construction.
- `with { Prop = newValue }` creates a modified **copy**; the original is untouched.
- `ReferenceEquals` is still `false` for two equal records — they're separate objects that happen to compare equal.
- Use `record` for pure data/values; use `class` for things with behavior and changing identity.
