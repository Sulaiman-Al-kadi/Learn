## Hint 1
Constructor injection is a one-liner pattern: store the interface in a `private readonly` field inside the constructor.
```csharp
private readonly INotifier _notifier;
public OrderProcessor(INotifier notifier) { _notifier = notifier; }
```

## Hint 2
`Complete` just delegates: `_notifier.Send($"Order {orderId} completed");`

## Hint 3
`PercentageDiscount` needs its own field for the percent, set in its constructor, then used in `Apply`:
```csharp
private readonly double _percent;
public PercentageDiscount(double percent) { _percent = percent; }
public double Apply(double subtotal) => subtotal - subtotal * _percent / 100;
```

## Hint 4
`Checkout.CalculateTotal` is one line: `return strategy.Apply(subtotal);` — no need to know or check which concrete strategy it is.
