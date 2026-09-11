## Hint 1
`Describe()` on each class is a one-line expression-bodied method matching the exact format in the README — copy it carefully, including the colon and spacing.

## Hint 2
`Total` is the accumulator pattern from Module 1: `double total = 0; foreach (IPayable p in payables) { total += p.Amount; } return total;`

## Hint 3
Both `Invoice` and `Employee` need `: IPayable` after their class name — that's what makes them fit into a `List<IPayable>` together despite being otherwise unrelated classes.
