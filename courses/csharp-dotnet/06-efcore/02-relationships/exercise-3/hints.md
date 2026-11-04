## Hint 1
Load the orders with `Include` first so that you can count them before removing the customer.

## Hint 2
`a.Orders.Sum(o => o.Total)` can be used inside a `Where`. `Total` is a `double` because SQLite can't sum `decimal`.
