# Exercise — Order analytics toolkit

Write six methods in `OrderTools.cs`, inside a static class `OrderTools`. The `Order` record is already given — don't change it.

```csharp
public record Order(string Customer, string Product, double Amount);
```

## Methods (LINQ only, no manual `foreach`)

| Method | Does |
|---|---|
| `double TotalRevenue(List<Order> orders)` | sum of every `Amount` |
| `double AverageOrderAmount(List<Order> orders)` | average `Amount` |
| `int CountBigOrders(List<Order> orders, double threshold)` | how many orders have `Amount > threshold` |
| `bool AnyOrderFrom(List<Order> orders, string customer)` | does at least one order belong to `customer`? |
| `Order? FindFirstOrderFrom(List<Order> orders, string customer)` | the first order (in original list order) from `customer`, or `null` if none |
| `List<string> CustomersSortedByTotalSpent(List<Order> orders)` | every distinct customer name, ordered by their **total spend** (sum of their orders' `Amount`) descending |

## Rules
- `CustomersSortedByTotalSpent` needs `GroupBy` (group by `Customer`), then order the **groups** by each group's total (`g.Sum(o => o.Amount)`) descending, then `Select` each group's `Key`.
- `FindFirstOrderFrom` must return `null` (not throw) when nothing matches — pick the right LINQ method accordingly.
