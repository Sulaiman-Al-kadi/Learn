## Hint 1
Without `.Include(a => a.Books)`, navigation properties stay empty. That is the lazy-navigation trap.

## Hint 2
Setting `AuthorId` on the new book is enough to link it.

## Hint 3
`ToDictionaryAsync` runs after the data is loaded, so `a.Books.Count` there sees an empty list. `Select` the name and the count first, so the count happens in the SQL, then build the dictionary.
