# Exercise — Querying with LINQ

LINQ on a `DbSet` becomes SQL. Write queries, not loops. Each method should end in an async EF call such as `ToListAsync`.

1. `CheaperThanAsync`: titles under `max`, A to Z.
2. `SearchAsync`: books whose title contains `text`.
3. `AveragePriceAsync`: the average price, or 0 with no books.
