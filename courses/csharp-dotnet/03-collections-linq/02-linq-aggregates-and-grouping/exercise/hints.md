## Hint 1
`TotalRevenue`: `orders.Sum(o => o.Amount);`. `AverageOrderAmount`: same idea with `.Average(...)`. `CountBigOrders`: `orders.Count(o => o.Amount > threshold);`

## Hint 2
`AnyOrderFrom`: `orders.Any(o => o.Customer == customer);`. `FindFirstOrderFrom`: `orders.FirstOrDefault(o => o.Customer == customer);` — FirstOrDefault, not First, so it returns null instead of throwing when nothing matches.

## Hint 3
`CustomersSortedByTotalSpent` — three steps:
```csharp
return orders
    .GroupBy(o => o.Customer)
    .OrderByDescending(g => g.Sum(o => o.Amount))
    .Select(g => g.Key)
    .ToList();
```
Each `g` in the chain is one customer's group of orders — `g.Sum(o => o.Amount)` is that customer's total spend, used as the sort key.
