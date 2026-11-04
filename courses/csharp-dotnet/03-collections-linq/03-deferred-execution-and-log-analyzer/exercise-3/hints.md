## Hint 1
Split with `StringSplitOptions.RemoveEmptyEntries`, then `Select(w => w.ToLowerInvariant())`.

## Hint 2
After `GroupBy(w => w)`, `Select(g => (g.Key, g.Count()))` builds the tuples.
