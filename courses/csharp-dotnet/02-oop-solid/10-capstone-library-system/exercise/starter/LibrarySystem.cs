// Module 2 Capstone — Library Lending System
// See README.md for the full spec of every member below.

public class Book
{
    public string Isbn { get; }
    public string Title { get; }
    public string Author { get; }
    public bool IsAvailable { get; private set; } = true;

    public Book(string isbn, string title, string author)
    {
        throw new NotImplementedException();
    }

    public void MarkBorrowed()
    {
        throw new NotImplementedException();
    }

    public void MarkReturned()
    {
        throw new NotImplementedException();
    }
}

public class Member
{
    public string Id { get; }
    public string Name { get; }
    public const int MaxBooks = 3;

    // TODO: private List<string> field for borrowed ISBNs, exposed via IReadOnlyList<string> BorrowedIsbns
    public IReadOnlyList<string> BorrowedIsbns => throw new NotImplementedException();

    public Member(string id, string name)
    {
        throw new NotImplementedException();
    }

    public bool CanBorrowMore => throw new NotImplementedException();

    public void AddBorrowed(string isbn)
    {
        throw new NotImplementedException();
    }

    public void RemoveBorrowed(string isbn)
    {
        throw new NotImplementedException();
    }
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
    public double CalculateFee(int daysLate)
    {
        throw new NotImplementedException();
    }
}

public class NoLateFeePolicy : ILateFeePolicy
{
    public double CalculateFee(int daysLate)
    {
        throw new NotImplementedException();
    }
}

public class Library
{
    // TODO: private readonly ILateFeePolicy field, set via constructor
    // TODO: private catalog — a Dictionary<string, Book>

    public Library(ILateFeePolicy lateFeePolicy)
    {
        throw new NotImplementedException();
    }

    public void AddBook(Book book)
    {
        throw new NotImplementedException();
    }

    public Book? FindByIsbn(string isbn)
    {
        throw new NotImplementedException();
    }

    public void Borrow(Member member, string isbn)
    {
        throw new NotImplementedException();
    }

    public double Return(Member member, string isbn, int daysLate)
    {
        throw new NotImplementedException();
    }
}
