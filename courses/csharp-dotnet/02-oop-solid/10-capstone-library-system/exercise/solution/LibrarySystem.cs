// Module 2 Capstone — Library Lending System (solution)

public class Book
{
    public string Isbn { get; }
    public string Title { get; }
    public string Author { get; }
    public bool IsAvailable { get; private set; } = true;

    public Book(string isbn, string title, string author)
    {
        Isbn = isbn;
        Title = title;
        Author = author;
    }

    public void MarkBorrowed()
    {
        if (!IsAvailable) throw new InvalidOperationException("Book already borrowed");
        IsAvailable = false;
    }

    public void MarkReturned()
    {
        IsAvailable = true;
    }
}

public class Member
{
    public string Id { get; }
    public string Name { get; }
    public const int MaxBooks = 3;

    private readonly List<string> _borrowedIsbns = new List<string>();
    public IReadOnlyList<string> BorrowedIsbns => _borrowedIsbns;

    public Member(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public bool CanBorrowMore => _borrowedIsbns.Count < MaxBooks;

    public void AddBorrowed(string isbn) => _borrowedIsbns.Add(isbn);

    public void RemoveBorrowed(string isbn) => _borrowedIsbns.Remove(isbn);
}

public class BookNotAvailableException : Exception
{
    public BookNotAvailableException(string message) : base(message) { }
}

public class BorrowLimitExceededException : Exception
{
    public BorrowLimitExceededException(string message) : base(message) { }
}

public interface ILateFeePolicy
{
    double CalculateFee(int daysLate);
}

public class StandardLateFeePolicy : ILateFeePolicy
{
    public double CalculateFee(int daysLate) => daysLate <= 0 ? 0 : daysLate * 0.5;
}

public class NoLateFeePolicy : ILateFeePolicy
{
    public double CalculateFee(int daysLate) => 0;
}

public class Library
{
    private readonly ILateFeePolicy _lateFeePolicy;
    private readonly Dictionary<string, Book> _catalog = new Dictionary<string, Book>();

    public Library(ILateFeePolicy lateFeePolicy)
    {
        _lateFeePolicy = lateFeePolicy;
    }

    public void AddBook(Book book)
    {
        _catalog[book.Isbn] = book;
    }

    public Book? FindByIsbn(string isbn)
    {
        return _catalog.TryGetValue(isbn, out Book? book) ? book : null;
    }

    public void Borrow(Member member, string isbn)
    {
        Book book = FindByIsbn(isbn) ?? throw new ArgumentException("Book not found");
        if (!book.IsAvailable) throw new BookNotAvailableException($"'{book.Title}' is not available");
        if (!member.CanBorrowMore) throw new BorrowLimitExceededException($"{member.Name} has reached the borrow limit");

        book.MarkBorrowed();
        member.AddBorrowed(isbn);
    }

    public double Return(Member member, string isbn, int daysLate)
    {
        Book book = FindByIsbn(isbn) ?? throw new ArgumentException("Book not found");

        book.MarkReturned();
        member.RemoveBorrowed(isbn);
        return _lateFeePolicy.CalculateFee(daysLate);
    }
}
