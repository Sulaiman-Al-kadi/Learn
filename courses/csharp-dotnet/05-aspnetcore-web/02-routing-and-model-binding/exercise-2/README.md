# Exercise — Books API

Add two endpoints over the `books` list.

1. `GET /books/{id:int}`: the book, or `404`.
2. `GET /books?author=...`: books by that author, ignoring case. With no `author`, all books.

A non-numeric id such as `/books/abc` should not match, which the route constraint gives you for free.
