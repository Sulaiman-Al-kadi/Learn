# Exercise — Money record

Write a `Money` record in `Money.cs`.

## Declaration
```csharp
public record Money(decimal Amount, string Currency);
```
(Start from this — you add methods to the record body, same as the `Money` example in the lesson.)

## Methods
- `Money Add(Money other)` — if `other.Currency != Currency`, throw `new ArgumentException("Currency mismatch")`. Otherwise return a **new** `Money` (use `with`) with `Amount` increased by `other.Amount`, same `Currency`. Must not modify `this`.
- `Money ApplyDiscount(decimal percent)` — return a new `Money` (use `with`) whose `Amount` is reduced by `percent`%, e.g. `100m` with `10` becomes `90m`. Must not modify `this`.

## Rules
- Use `with` for both methods — never construct a `new Money(...)` from scratch inside them (that would work too, but `with` is the point of this exercise).
- Because `Money` is a record, the tests compare results directly with `==`/`Assert.Equal` against a freshly-built expected `Money` — that only works because of the value equality records give you for free (lesson 05).
