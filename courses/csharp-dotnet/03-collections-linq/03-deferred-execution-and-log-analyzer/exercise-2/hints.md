## Hint 1
A query with no `ToList()` stays deferred. Adding `ToList()` runs it immediately.

## Hint 2
Increment `calls` inside the lambda. Use `.Count()` twice to walk the query twice.
