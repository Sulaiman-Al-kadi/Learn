## Hint 1
`_context.Books.Where(...).OrderBy(...).Select(...).ToListAsync()` has the same shape as LINQ to objects.

## Hint 2
Check `await _context.Books.AnyAsync()` first: `AverageAsync` throws on an empty table.

## Hint 3
SQLite can't order or average `decimal` columns, so `Price` here is a `double`.
