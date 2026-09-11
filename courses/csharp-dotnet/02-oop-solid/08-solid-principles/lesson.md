# Module 2, Lesson 08 — SOLID Principles

You now have every OOP building block: classes, inheritance, interfaces, generics, exceptions. **SOLID** is five principles about *how* to combine them so code stays easy to change as it grows — this is the difference between code that's merely working and code that's well-designed. Every letter is grounded in what you've already learned.

## S — Single Responsibility Principle (SRP)

**A class should have one reason to change.**

```csharp
// BAD — one class doing three unrelated jobs
public class Report
{
    public string GenerateText(List<string> lines) => string.Join("\n", lines);

    public void SaveToFile(string content, string path)
    {
        File.WriteAllText(path, content);
    }

    public void SendEmail(string content, string address)
    {
        // ... email sending logic ...
    }
}
```

If the email logic changes, you touch `Report`. If the save format changes, you touch `Report`. If the text layout changes, you touch `Report` again. Three unrelated reasons for one class to change means three ways for changes to accidentally break each other.

```csharp
// GOOD — split by responsibility
public class ReportGenerator
{
    public string GenerateText(List<string> lines) => string.Join("\n", lines);
}

public class FileSaver
{
    public void Save(string content, string path) => File.WriteAllText(path, content);
}

public class EmailSender
{
    public void Send(string content, string address) { /* ... */ }
}
```

Each class now has exactly one job, and one reason to ever be modified.

## O — Open/Closed Principle (OCP)

**Open for extension, closed for modification** — you should be able to add new behavior without editing existing, already-working code.

```csharp
// BAD — every new shape means editing this method again
public double Area(object shape)
{
    if (shape is Circle c) return Math.PI * c.Radius * c.Radius;
    if (shape is Rectangle r) return r.Width * r.Height;
    // adding Triangle means coming back here and adding another `if`
    throw new ArgumentException("Unknown shape");
}
```

You already have the fix from lesson 03 — polymorphism:

```csharp
// GOOD — Shape.Area() is virtual; adding Triangle needs ZERO changes here
public double TotalArea(List<Shape> shapes)
{
    double total = 0;
    foreach (Shape s in shapes) total += s.Area();
    return total;
}
```

Add a new `Triangle : Shape` with its own `override Area()`, and `TotalArea` handles it automatically — the existing, tested code is never touched. This is exactly what lesson 03's polymorphism and lesson 04's interfaces were building toward.

## L — Liskov Substitution Principle (LSP)

**A subclass must be usable anywhere its base class is expected, without breaking things.**

```csharp
public class Rectangle
{
    public virtual double Width { get; set; }
    public virtual double Height { get; set; }
    public double Area => Width * Height;
}

// BAD — Square "is-a" Rectangle by inheritance, but breaks the substitution
public class Square : Rectangle
{
    public override double Width
    {
        get => base.Width;
        set { base.Width = value; base.Height = value; }   // sneaky side effect!
    }
}
```

```csharp
Rectangle r = new Square();
r.Width = 5;
r.Height = 10;                          // caller expects a 5x10 rectangle...
Console.WriteLine(r.Area);              // ...but gets 100, because Height silently followed Width!
```

Any code written correctly against `Rectangle` breaks when handed a `Square`, because `Square` secretly violates the assumption "setting Width doesn't affect Height." LSP says: if it doesn't behave like the base type promises, it shouldn't inherit from it — model it differently instead (e.g. both implement a shared `IShape` interface, no inheritance between them).

## I — Interface Segregation Principle (ISP)

**Don't force a class to implement methods it doesn't need.**

```csharp
// BAD — a fat interface forces irrelevant methods on every implementer
public interface IWorker
{
    void Work();
    void Eat();
}

public class RobotWorker : IWorker
{
    public void Work() { /* ... */ }
    public void Eat() => throw new NotImplementedException();   // robots don't eat — forced to stub this out
}
```

```csharp
// GOOD — split into focused interfaces (lesson 04: a class can implement several)
public interface IWorkable { void Work(); }
public interface IFeedable { void Eat(); }

public class RobotWorker : IWorkable
{
    public void Work() { /* ... */ }        // only implements what actually applies
}

public class HumanWorker : IWorkable, IFeedable
{
    public void Work() { /* ... */ }
    public void Eat() { /* ... */ }
}
```

Small, focused interfaces (lesson 04's "capability" idea) let each class implement exactly what it genuinely supports.

## D — Dependency Inversion Principle (DIP)

**Depend on abstractions (interfaces), not on concrete classes.**

```csharp
// BAD — OrderProcessor is permanently welded to EmailSender
public class EmailSender
{
    public void Send(string message) => Console.WriteLine($"Email: {message}");
}

public class OrderProcessor
{
    private EmailSender _sender = new EmailSender();   // hard-coded dependency

    public void Complete(string order)
    {
        _sender.Send($"Order {order} completed");
    }
}
```

To ever notify by SMS instead — or in a test, avoid sending anything real — you'd have to edit `OrderProcessor` itself.

```csharp
// GOOD — depend on an interface (lesson 04), not a specific class
public interface INotifier
{
    void Send(string message);
}

public class EmailNotifier : INotifier
{
    public void Send(string message) => Console.WriteLine($"Email: {message}");
}

public class SmsNotifier : INotifier
{
    public void Send(string message) => Console.WriteLine($"SMS: {message}");
}

public class OrderProcessor
{
    private readonly INotifier _notifier;

    public OrderProcessor(INotifier notifier)      // the dependency is INJECTED, not created inside
    {
        _notifier = notifier;
    }

    public void Complete(string order)
    {
        _notifier.Send($"Order {order} completed");
    }
}
```

```csharp
var withEmail = new OrderProcessor(new EmailNotifier());
var withSms = new OrderProcessor(new SmsNotifier());
```

`OrderProcessor` no longer cares *how* notification happens — only that whatever it's handed can `Send(...)`. This pattern — passing dependencies into a constructor instead of creating them inside — is called **Dependency Injection**, and it's the backbone of how ASP.NET Core is built (Module 5). You already have every tool it needs: interfaces (lesson 04) and constructors (lesson 01).

## Why this all matters going forward

SOLID isn't a checklist to memorize and recite — it's a description of *why* interfaces, polymorphism, and constructors work the way they do. Every principle here maps directly to something you already learned:

| Principle | Built from |
|---|---|
| SRP | Just class design discipline — one job per class |
| OCP | `virtual`/`override` polymorphism (lesson 03) |
| LSP | Correct use of inheritance (lesson 03) — don't inherit if it breaks the base's promises |
| ISP | Small interfaces (lesson 04) |
| DIP | Interfaces (lesson 04) + constructors (lesson 01), combined as constructor injection |

## Summary
- **S**RP: one class, one responsibility, one reason to change.
- **O**CP: extend via new classes/overrides, don't modify working code.
- **L**SP: a subclass must honor its base class's behavior contract, not secretly break it.
- **I**SP: small, focused interfaces beat one giant one that forces irrelevant methods.
- **D**IP: depend on interfaces, inject concrete implementations through the constructor — don't `new` your dependencies inside a class.
