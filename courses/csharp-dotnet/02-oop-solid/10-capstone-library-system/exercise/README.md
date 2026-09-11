# Capstone — Library Lending System

Write everything in `LibrarySystem.cs`: two data classes, two custom exceptions, an interface with two implementations, and one class tying it together.

## `Book`
- `string Isbn { get; }`, `string Title { get; }`, `string Author { get; }` — set once via the constructor, never after (no public setter at all — `get` only).
- `bool IsAvailable { get; private set; }` — starts `true`.
- Constructor `Book(string isbn, string title, string author)`.
- `void MarkBorrowed()` — if already `!IsAvailable`, `throw new InvalidOperationException("Book already borrowed")`. Otherwise set `IsAvailable = false`.
- `void MarkReturned()` — set `IsAvailable = true`.

## `Member`
- `string Id { get; }`, `string Name { get; }` — set via constructor.
- `public const int MaxBooks = 3;`
- Internally track borrowed ISBNs in a `private List<string>`; expose them as `IReadOnlyList<string> BorrowedIsbns`.
- `bool CanBorrowMore` — computed property: `true` while the member has borrowed fewer than `MaxBooks`.
- `void AddBorrowed(string isbn)` / `void RemoveBorrowed(string isbn)` — add/remove from the internal list.

## Custom exceptions
- `class BookNotAvailableException : Exception` with a `(string message) : base(message)` constructor.
- `class BorrowLimitExceededException : Exception`, same pattern.

## `ILateFeePolicy` (interface)
```csharp
public interface ILateFeePolicy
{
    double CalculateFee(int daysLate);
}
```
- `StandardLateFeePolicy : ILateFeePolicy` — `CalculateFee` returns `0` if `daysLate <= 0`, otherwise `daysLate * 0.5`.
- `NoLateFeePolicy : ILateFeePolicy` — `CalculateFee` always returns `0`.

## `Library`
- Constructor `Library(ILateFeePolicy lateFeePolicy)` — stored, injected (DIP — don't `new` a policy inside `Library`).
- Internal catalog: `private Dictionary<string, Book>` keyed by ISBN.
- `void AddBook(Book book)` — adds it to the catalog (key = `book.Isbn`).
- `Book? FindByIsbn(string isbn)` — returns the book, or `null` if not in the catalog.
- `void Borrow(Member member, string isbn)`:
  1. Look the book up; if not found, `throw new ArgumentException("Book not found")`.
  2. If `!book.IsAvailable`, `throw new BookNotAvailableException($"'{book.Title}' is not available")`.
  3. If `!member.CanBorrowMore`, `throw new BorrowLimitExceededException($"{member.Name} has reached the borrow limit")`.
  4. Otherwise: `book.MarkBorrowed()` and `member.AddBorrowed(isbn)`.
- `double Return(Member member, string isbn, int daysLate)`:
  1. Look the book up; if not found, `throw new ArgumentException("Book not found")`.
  2. `book.MarkReturned()`, `member.RemoveBorrowed(isbn)`.
  3. Return `lateFeePolicy.CalculateFee(daysLate)` (using whichever policy was injected in the constructor).

## Rules
- `Library` must never construct a concrete `ILateFeePolicy` itself — only use the one passed into its constructor.
- Don't let anything outside `Book`/`Member` mutate their internal state directly — always go through the methods above.

Check with the beaker — this exercise has a large test suite exercising every class. Reading `Tests.cs` afterward is worthwhile; it's a good example of a realistic test file for a small multi-class system.
