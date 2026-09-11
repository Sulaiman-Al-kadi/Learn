# Exercise — DIP notifier + OCP discount strategy

Write everything in `Solid.cs`. Two independent parts, each demonstrating one principle.

## Part 1 — Dependency Inversion

```csharp
public interface INotifier
{
    void Send(string message);
}

public class OrderProcessor
{
    // TODO
}
```

- `OrderProcessor` takes an `INotifier` through its **constructor** and stores it (constructor injection, exactly like the lesson's `OrderProcessor`/`INotifier` example).
- `void Complete(string orderId)` — calls `notifier.Send($"Order {orderId} completed")`.

The test file supplies its **own** `INotifier` implementation to verify what was sent — that's only possible because `OrderProcessor` depends on the interface, not a concrete class. This is DIP in action.

## Part 2 — Open/Closed via a strategy interface

```csharp
public interface IDiscountStrategy
{
    double Apply(double subtotal);
}
```

- `NoDiscount : IDiscountStrategy` — `Apply` returns `subtotal` unchanged.
- `PercentageDiscount : IDiscountStrategy` — constructor `PercentageDiscount(double percent)`; `Apply` returns `subtotal` reduced by `percent`%, e.g. 100 with 10% → 90.
- `Checkout` (static class) — `static double CalculateTotal(double subtotal, IDiscountStrategy strategy)` returns `strategy.Apply(subtotal)`.

Because `Checkout.CalculateTotal` depends only on the `IDiscountStrategy` interface, the test file can define a **brand-new** discount strategy of its own and pass it in — `Checkout` needs zero changes to support it. That's the Open/Closed Principle: open for extension (new strategies), closed for modification (`Checkout` itself never changes).

## Rules
- Don't have `OrderProcessor` create its own `INotifier` internally (`new EmailNotifier()` inside the class) — it must come in through the constructor.
- `Checkout.CalculateTotal` must not contain any `if`/`switch` checking which strategy it received — it should just call `strategy.Apply(subtotal)` and let polymorphism handle the rest.
