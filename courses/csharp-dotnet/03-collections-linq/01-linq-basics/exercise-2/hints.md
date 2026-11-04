## Hint 1
`Where` filters, `OrderBy` sorts, and `Select` turns each product into the value you want. Do them in that order.

## Hint 2
`OrderBy(p => p.Price).ThenBy(p => p.Name)` gives you the two-level sort.
