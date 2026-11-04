## Hint 1
`Select` can run before `Where`: trim first, then test the trimmed text.

## Hint 2
`OrderByDescending(w => w.Length).ThenBy(w => w)` sorts by length, then by the word itself.
