## Hint 1
`ids.Select(fetch)` starts one task per id. Materialize it with `.ToList()`.

## Hint 2
`await Task.WhenAll(tasks)` gives you an array of results in the same order as the tasks.
