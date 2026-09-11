## Hint 1
`Book`'s constructor just assigns the three readonly-getter properties. `MarkBorrowed`/`MarkReturned` are the same guard-clause pattern from lesson 07's BankAccount.

## Hint 2
`Member`'s borrowed list: `private readonly List<string> _borrowedIsbns = new List<string>();` then `public IReadOnlyList<string> BorrowedIsbns => _borrowedIsbns;` (lesson 02's read-only exposure pattern). `CanBorrowMore => _borrowedIsbns.Count < MaxBooks;`

## Hint 3
`Library`'s catalog: `private readonly Dictionary<string, Book> _catalog = new Dictionary<string, Book>();`. `AddBook`: `_catalog[book.Isbn] = book;`. `FindByIsbn`: `_catalog.TryGetValue(isbn, out Book? book) ? book : null;`

## Hint 4
`Borrow` and `Return` are each a short sequence of guard clauses (throwing the right exception) followed by the actual state changes — re-read the README's numbered steps and translate them almost line for line.

## Hint 5
Remember DIP: `Library`'s constructor just stores whatever `ILateFeePolicy` it's given in a field — it never writes `new StandardLateFeePolicy()` itself anywhere.
