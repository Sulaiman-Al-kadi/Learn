## Hint 1
`GroupBy(e => e.Subject).ToDictionary(g => g.Key, g => g.Average(e => e.Score))` builds the dictionary.

## Hint 2
For paging, skip `(page - 1) * size` items, then take `size`.

## Hint 3
`OrderByDescending(...).FirstOrDefault()` gives `null` on an empty list, and `?.` reads a property safely.
