# Module 2, Lesson 01 — Classes & Objects

Everything in Module 1 was **procedural**: variables and methods sitting loose in a file. Now we start **object-oriented programming (OOP)** — bundling data and the behavior that operates on it together, into one unit: a **class**.

## A class is a blueprint; an object is one instance of it

```csharp
public class Dog
{
    public string Name;
    public int Age;

    public void Bark()
    {
        Console.WriteLine($"{Name} says Woof!");
    }
}
```

`Dog` is the **blueprint** — it says "every dog has a Name, an Age, and can Bark." It doesn't create any actual dog. To get a real dog, you **instantiate** it with `new`:

```csharp
Dog rex = new Dog();
rex.Name = "Rex";
rex.Age = 3;
rex.Bark();               // Rex says Woof!

Dog milo = new Dog();
milo.Name = "Milo";
milo.Age = 1;
milo.Bark();               // Milo says Woof!
```

`rex` and `milo` are two separate **objects** (also called **instances**) of the same class. Each has its own independent copy of `Name` and `Age` — changing `rex.Name` does not affect `milo.Name`. This is the whole point: one blueprint, many independent things built from it.

`Name` and `Age` here are **fields** — variables that belong to an object. `Bark` is an **instance method** — a method that operates on one particular object's data.

## `this` — referring to "the current object"

Inside an instance method, `this` means "the object this method was called on":

```csharp
public class Dog
{
    public string Name;

    public void Bark()
    {
        Console.WriteLine($"{this.Name} says Woof!");   // this.Name is the same as just Name here
    }
}
```

You'll mostly see `this` used to resolve a naming clash — very common in constructors (next).

## Constructors — setting up a new object

Typing `rex.Name = "Rex"; rex.Age = 3;` after every `new Dog()` is repetitive and easy to forget. A **constructor** lets you require that information at creation time:

```csharp
public class Dog
{
    public string Name;
    public int Age;

    public Dog(string name, int age)      // same name as the class, no return type
    {
        this.Name = name;                 // `this.Name` = the field; `name` = the parameter
        this.Age = age;
    }

    public void Bark()
    {
        Console.WriteLine($"{Name} says Woof!");
    }
}
```

```csharp
Dog rex = new Dog("Rex", 3);     // must supply both — the compiler enforces it
```

`this.Name = name;` — without `this.`, `Name = name;` would still work here because the parameter is lowercase and the field is uppercase, but when they share a name (`this.name = name;`) `this.` is what tells C# "the field, not the parameter."

If you write no constructor at all, C# silently gives you a free parameterless one (`new Dog()`) — but the moment you write **any** constructor yourself, that free one disappears, and callers must use one you defined.

## Multiple objects, independent state

This is worth staring at until it clicks — it's the core idea of OOP:

```csharp
Dog a = new Dog("Rex", 3);
Dog b = new Dog("Rex", 3);          // same values, but...

Console.WriteLine(a == b);          // False — classes compare by reference (Module 1 memory notes!)
a.Age = 10;
Console.WriteLine(b.Age);           // 1... unaffected. a and b are separate objects on the heap.
```

## Methods that use and change an object's own fields

```csharp
public class BankAccount
{
    public double Balance;

    public BankAccount(double startingBalance)
    {
        Balance = startingBalance;
    }

    public void Deposit(double amount)
    {
        Balance += amount;
    }

    public bool Withdraw(double amount)
    {
        if (amount > Balance) return false;   // not enough money — refuse
        Balance -= amount;
        return true;
    }
}
```

```csharp
BankAccount acc = new BankAccount(100);
acc.Deposit(50);              // Balance is now 150
bool ok = acc.Withdraw(200);  // false — refused, Balance still 150
Console.WriteLine(acc.Balance);
```

Every method here reads and modifies **that particular object's** `Balance`. Two different `BankAccount` objects never interfere with each other.

## Why this matters

Procedural code (Module 1) forces you to pass every related piece of data around separately: `Deposit(balance, amount)`, `Withdraw(balance, amount)`... and nothing stops you from accidentally mixing up whose balance you're modifying. A class packages the data (`Balance`) with the operations that are allowed on it (`Deposit`, `Withdraw`) into one unit. This is the foundation everything else in OOP builds on.

## Summary
- `class` = blueprint. `new ClassName(...)` creates an **object** (instance).
- **Fields** hold an object's data; **instance methods** operate on that data.
- Each object has its own independent copy of its fields.
- A **constructor** (same name as the class, no return type) runs when you `new` an object — use it to require the data an object needs to be valid.
- `this` refers to the current object, most often used to disambiguate a field from a same-named parameter.
