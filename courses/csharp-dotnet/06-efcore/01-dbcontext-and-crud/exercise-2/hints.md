## Hint 1
Every change ends with `await _context.SaveChangesAsync()`.

## Hint 2
`await _context.Books.FindAsync(id)` returns the entity or `null`. Change tracking notices property edits for you.
