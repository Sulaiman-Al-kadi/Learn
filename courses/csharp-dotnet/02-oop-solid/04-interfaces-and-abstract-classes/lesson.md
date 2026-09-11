# Module 2, Lesson 04 — Interfaces & Abstract Classes

Inheritance (`class Dog : Animal`) models "Dog **is a kind of** Animal." Often, though, you want to say "this class **can do X**" without forcing a specific inheritance chain — a `Duck` and an `Airplane` can both `Fly()`, yet they have nothing else in common and definitely shouldn't inherit from the same base class. That's what **interfaces** are for.

## Declaring and implementing an interface

```csharp
public interface IFlyable
{
    void Fly();
}

public class Duck : IFlyable
{
    public void Fly()
    {
        Console.WriteLine("The duck flaps its wings and flies.");
    }
}

public class Airplane : IFlyable
{
    public void Fly()
    {
        Console.WriteLine("The airplane's engines roar to life.");
    }
}
```

An interface is a **contract**: it declares *what* members a class must have (names, parameters, return types) but never *how* they work — no bodies, no fields. Convention: interface names start with a capital `I`.

`: IFlyable` means "I promise to provide everything this interface requires." If `Duck` forgets to implement `Fly()`, it's a **compile error** — the contract isn't optional.

## Why: polymorphism without inheritance

Just like a base class reference in lesson 03, an interface reference lets you treat unrelated types uniformly:

```csharp
List<IFlyable> fliers = new List<IFlyable> { new Duck(), new Airplane() };

foreach (IFlyable f in fliers)
{
    f.Fly();       // each runs its own implementation
}
```

`Duck` and `Airplane` share **no** common base class (other than `object`), yet this works perfectly, because both fulfill the `IFlyable` contract.

## A class can implement multiple interfaces

Unlike inheriting from a class (you can only have **one** base class), a class can implement as many interfaces as it needs:

```csharp
public interface ISwimmable
{
    void Swim();
}

public class Duck : IFlyable, ISwimmable
{
    public void Fly() => Console.WriteLine("Duck flies.");
    public void Swim() => Console.WriteLine("Duck swims.");
}
```

This is deliberate: "can fly," "can swim," "can be compared," "can be disposed" are independent *capabilities* — a real-world thing often has several at once, and interfaces model that cleanly.

## Interfaces can require properties too

```csharp
public interface IPayable
{
    double Amount { get; }
    string Describe();
}

public class Invoice : IPayable
{
    public double Amount { get; set; }
    public string Describe() => $"Invoice for {Amount:C}";
}

public class Employee : IPayable
{
    public double Amount { get; set; }     // their salary
    public string Name { get; set; } = "";
    public string Describe() => $"{Name}'s pay: {Amount:C}";
}
```

```csharp
List<IPayable> payables = new List<IPayable> { new Invoice { Amount = 500 }, new Employee { Name = "Sara", Amount = 3000 } };
double total = 0;
foreach (IPayable p in payables)
{
    Console.WriteLine(p.Describe());
    total += p.Amount;
}
```

Notice: `Invoice` and `Employee` are completely unrelated classes. `IPayable` is the *only* thing that lets you process them together in one loop.

## Abstract classes — a hybrid

An **abstract class** is like a base class that can't be instantiated directly, and can mix fully-implemented members with ones that force derived classes to provide their own:

```csharp
public abstract class Shape
{
    public string Name { get; set; } = "";

    public abstract double Area();                              // no body — MUST be overridden

    public string Describe() => $"{Name}: {Area():0.00}";       // full implementation, shared by all
}

public class Circle : Shape
{
    public double Radius { get; set; }
    public override double Area() => Math.PI * Radius * Radius;   // required
}
```

```csharp
Shape s = new Shape();     // ERROR: cannot create an instance of an abstract class
Shape c = new Circle();    // fine — Circle is concrete (fully implemented)
```

An `abstract` method (no body, ends with `;`) forces every non-abstract derived class to `override` it — stronger than `virtual`, which is optional to override.

## Interface vs. abstract class — which one?

| | Interface | Abstract class |
|---|---|---|
| Multiple per class? | Yes, as many as needed | No — only one base class |
| Can hold shared, working code? | Only default implementations (advanced, rare) | Yes, freely |
| Can hold fields? | No | Yes |
| Models | "Can do X" (a capability) | "Is a kind of X" (with shared groundwork) |
| Instantiable? | Never | Never (but its concrete subclasses are) |

**Rule of thumb:** use an **interface** to describe a capability that unrelated classes might share (`IComparable`, `IDisposable`, `IPayable`). Use an **abstract class** when there's real shared code/state across a family of related types, and you still want to force each to fill in specifics (`Shape` → `Area()`).

They can combine: a class can inherit **one** abstract class **and** implement **any number of** interfaces at the same time: `public class Circle : Shape, IComparable<Shape>`.

## Interfaces you'll meet built into .NET

You won't just write your own — the standard library is full of them:
- `IEnumerable<T>` — "can be iterated with `foreach`" (every `List<T>`, array, etc. implements this — that's *why* `foreach` works on all of them uniformly).
- `IComparable<T>` — "can be compared/sorted."
- `IDisposable` — "has cleanup to run" (`using`, from Module 1's memory lesson).

## Summary
- **Interface**: a contract of member signatures, no implementation. `class X : IY` promises X provides everything `IY` requires.
- A class can implement **many** interfaces, but inherit from only **one** class.
- Interface references (`List<IPayable>`) give you polymorphism across otherwise-unrelated classes.
- **Abstract class**: can't be instantiated; mixes required (`abstract`) members with shared, fully-implemented ones.
- Interface = "can do." Abstract class = "is a kind of, with shared groundwork."
