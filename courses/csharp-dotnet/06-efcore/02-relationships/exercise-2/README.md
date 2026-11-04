# Exercise — Authors and books

Work with a one-to-many relationship. `Author` has many `Book`s.

1. `AddBookAsync`: add a book to an existing author. Return false when the author doesn't exist.
2. `GetWithBooksAsync`: the author with `Books` loaded, or `null`.
3. `BookCountsAsync`: a dictionary of author name to number of books.
