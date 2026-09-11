# Module 2, Lesson 03 — Inheritance & Polymorphism

Many classes share common structure: a `Circle` and a `Rectangle` are both "shapes" with an area. **Inheritance** lets one class reuse and extend another's members instead of duplicating them.

## `: BaseClass` — inheriting

```csharp
public class Animal
{
    public string Name { get; set; } = "";

    public void Eat()
    {
        Console.WriteLine($"{Name} is eating.");
    }
}

public class Dog : Animal      // Dog INHERITS FROM Animal
{
    public void Fetch()
    {
        Console.WriteLine($"{Name} fetches the ball!");
    }
}
```

`Dog` is the **derived class** (or subclass); `Animal` is the **base class** (or superclass). Every `Dog` automatically has everything `Animal` has, plus its own additions:

```csharp
Dog d = new Dog();
d.Name = "Rex";     // inherited from Animal
d.Eat();             // inherited from Animal
d.Fetch();           // Dog's own
```

This models an **"is-a" relationship**: a `Dog` *is an* `Animal`. (Contrast with lesson 01's classes, which just *have* fields — that's not inheritance.)

## Constructors and `base(...)`

A derived class's constructor must ensure the base class gets initialized too:

```csharp
public class Animal
{
    public string Name { get; set; }

    public Animal(string name)
    {
        Name = name;
    }
}

public class Dog : Animal
{
    public string Breed { get; set; }

    public Dog(string name, string breed) : base(name)   // calls Animal's constructor first
    {
        Breed = breed;
    }
}
```

`: base(name)` runs `Animal`'s constructor with `name` **before** `Dog`'s own constructor body executes. If `Animal` has no parameterless constructor, every derived class's constructor **must** explicitly call `base(...)` with the right arguments — there's no way around it, because an `Animal` part must always be fully constructed.

## `virtual` and `override` — changing inherited behavior

By default, a derived class can't change how an inherited method behaves — only add new members. To allow a derived class to **replace** a method's implementation, mark it `virtual` in the base class and `override` in the derived one:

```csharp
public class Animal
{
    public string Name { get; set; } = "";

    public virtual string Speak()
    {
        return $"{Name} makes a sound.";
    }
}

public class Dog : Animal
{
    public override string Speak()
    {
        return $"{Name} says Woof!";
    }
}

public class Cat : Animal
{
    public override string Speak()
    {
        return $"{Name} says Meow!";
    }
}
```

Without `virtual`/`override`, every `Animal` (and everything pretending to be one) would say "makes a sound" — there'd be no way for `Dog` to specialize it.

## Polymorphism — one type, many behaviors

This is where it pays off. You can put different derived types in a collection **typed as the base class**, and each one runs its *own* overridden behavior automatically:

```csharp
List<Animal> zoo = new List<Animal>
{
    new Dog { Name = "Rex" },
    new Cat { Name = "Mimi" },
    new Animal { Name = "Generic Creature" },
};

foreach (Animal a in zoo)
{
    Console.WriteLine(a.Speak());
}
```
```
Rex says Woof!
Mimi says Meow!
Generic Creature makes a sound.
```

The loop variable is typed `Animal`, but each call to `Speak()` runs the **actual object's** version — `Dog`'s if it's a Dog, `Cat`'s if it's a Cat. This is **polymorphism** ("many forms"): the same call, `a.Speak()`, does different things depending on the real object underneath. It's what lets you write one function that handles an entire family of related types without an `if`/`else if` chain checking "is this a Dog? Is this a Cat?" for every one.

## `base.Method()` — calling the parent's version

Sometimes you want to *extend* the base behavior, not fully replace it:

```csharp
public class Dog : Animal
{
    public override string Speak()
    {
        string baseSound = base.Speak();          // call Animal's original Speak()
        return baseSound + " Specifically, Woof!";
    }
}
```

## `protected` — visible to this class and its subclasses

Recall `private` blocks everything outside the class. `protected` allows the class **and any class that inherits from it** to access a member, while still hiding it from the outside world:

```csharp
public class Animal
{
    protected int Age { get; set; }     // Dog can use Age; outside code cannot
}
```

## Checking the actual type — `is` and pattern matching

```csharp
foreach (Animal a in zoo)
{
    if (a is Dog dog)
    {
        dog.Fetch();       // Fetch only exists on Dog, not Animal — need the narrowed type first
    }
}
```
This is fine occasionally, but relying on it heavily (a long chain of `is Dog` / `is Cat` checks) usually means you should have made `Fetch`-like behavior a `virtual` method on `Animal` instead — that's the whole point of polymorphism, and it's what SOLID's Open/Closed principle (Module 2, later lesson) is built on.

## `sealed` — preventing further inheritance

```csharp
public sealed class FinalDog : Animal
{
    // no class may inherit from FinalDog
}
```
Rare, but useful to explicitly say "this class is not meant to be extended further."

## Summary
- `class Derived : Base` — inheritance, an "is-a" relationship. Derived gets everything Base has, plus its own members.
- `: base(...)` in a constructor initializes the base part first.
- `virtual` (base) + `override` (derived) let a derived class replace a method's behavior.
- **Polymorphism**: a collection of the base type can hold any derived type; calling a virtual method runs each object's own version automatically.
- `base.Method()` calls the parent's implementation from within an override.
- `protected` — accessible to this class and its subclasses, hidden from everyone else.
- `is Type variable` narrows to a derived type when you truly need type-specific behavior — but prefer polymorphism (a `virtual` method) when you can.
