## Hint 1
`Add`: check the guard first, then return the sum as a new record:
```csharp
if (other.Currency != Currency) throw new ArgumentException("Currency mismatch");
return this with { Amount = Amount + other.Amount };
```

## Hint 2
`ApplyDiscount`: the amount to subtract is `Amount * percent / 100`:
```csharp
return this with { Amount = Amount - Amount * percent / 100 };
```

## Hint 3
`this with { ... }` inside a record's own method works exactly like `someRecord with { ... }` from the outside — it copies every property of the current instance except the ones you override.
