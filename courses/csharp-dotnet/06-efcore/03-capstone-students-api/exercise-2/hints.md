## Hint 1
`(AppDbContext db, NewNote body) => ...` injects the context and binds the body.

## Hint 2
Make the lambdas `async` and return `Results.Created($"/notes/{note.Id}", note)`.
