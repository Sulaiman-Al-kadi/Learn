# Exercise — IPayable interface

Write everything in `Payroll.cs`:

## `IPayable` (interface)
```csharp
public interface IPayable
{
    double Amount { get; }
    string Describe();
}
```
(You can copy this part directly — the exercise is in the classes that implement it.)

## `Invoice : IPayable`
- `string ClientName { get; set; }`
- `double Amount { get; set; }`
- `Describe()` returns exactly `$"Invoice for {ClientName}: {Amount}"`, e.g. `"Invoice for Acme Co: 500"`.

## `Employee : IPayable`
- `string Name { get; set; }`
- `double Amount { get; set; }`
- `Describe()` returns exactly `$"{Name}'s pay: {Amount}"`, e.g. `"Sara's pay: 3000"`.

## `Payroll` (static class)
- `static double Total(List<IPayable> payables)` — loops through the list and sums every `Amount`. No LINQ — a plain loop.

## Rules
- `Invoice` and `Employee` share **no** base class — only the `IPayable` interface connects them. That's what lets a single `List<IPayable>` and a single `Total` method handle both.
- `Amount` in the interface only requires a getter (`{ get; }`) — but your classes can still provide a full `{ get; set; }`; that satisfies the interface just fine (a property with more capability than the interface requires is always acceptable).
