using Xunit;

public class BookTests
{
    [Fact]
    public void NewBook_IsAvailable()
    {
        var book = new Book("111", "Dune", "Frank Herbert");
        Assert.True(book.IsAvailable);
    }

    [Fact]
    public void MarkBorrowed_MakesUnavailable()
    {
        var book = new Book("111", "Dune", "Frank Herbert");
        book.MarkBorrowed();
        Assert.False(book.IsAvailable);
    }

    [Fact]
    public void MarkBorrowed_Twice_Throws()
    {
        var book = new Book("111", "Dune", "Frank Herbert");
        book.MarkBorrowed();
        Assert.Throws<InvalidOperationException>(() => book.MarkBorrowed());
    }

    [Fact]
    public void MarkReturned_MakesAvailableAgain()
    {
        var book = new Book("111", "Dune", "Frank Herbert");
        book.MarkBorrowed();
        book.MarkReturned();
        Assert.True(book.IsAvailable);
    }
}

public class MemberTests
{
    [Fact]
    public void NewMember_CanBorrowMore()
    {
        var member = new Member("M1", "Sara");
        Assert.True(member.CanBorrowMore);
        Assert.Empty(member.BorrowedIsbns);
    }

    [Fact]
    public void AddBorrowed_TracksIsbns()
    {
        var member = new Member("M1", "Sara");
        member.AddBorrowed("111");
        member.AddBorrowed("222");
        Assert.Equal(2, member.BorrowedIsbns.Count);
        Assert.Contains("111", member.BorrowedIsbns);
    }

    [Fact]
    public void CanBorrowMore_FalseAtLimit()
    {
        var member = new Member("M1", "Sara");
        member.AddBorrowed("1");
        member.AddBorrowed("2");
        member.AddBorrowed("3");
        Assert.False(member.CanBorrowMore);
    }

    [Fact]
    public void RemoveBorrowed_TakesItOffTheList()
    {
        var member = new Member("M1", "Sara");
        member.AddBorrowed("111");
        member.RemoveBorrowed("111");
        Assert.Empty(member.BorrowedIsbns);
    }
}

public class LateFeePolicyTests
{
    [Fact]
    public void StandardPolicy_NoDaysLate_IsFree()
    {
        Assert.Equal(0, new StandardLateFeePolicy().CalculateFee(0));
    }

    [Fact]
    public void StandardPolicy_ChargesPerDay()
    {
        Assert.Equal(2.0, new StandardLateFeePolicy().CalculateFee(4));
    }

    [Fact]
    public void NoFeePolicy_AlwaysZero()
    {
        Assert.Equal(0, new NoLateFeePolicy().CalculateFee(30));
    }
}

public class LibraryTests
{
    private static Book SampleBook(string isbn = "111") => new Book(isbn, "Dune", "Frank Herbert");

    [Fact]
    public void AddBook_ThenFindByIsbn_ReturnsIt()
    {
        var library = new Library(new StandardLateFeePolicy());
        library.AddBook(SampleBook());
        Assert.NotNull(library.FindByIsbn("111"));
        Assert.Equal("Dune", library.FindByIsbn("111")!.Title);
    }

    [Fact]
    public void FindByIsbn_NotFound_ReturnsNull()
    {
        var library = new Library(new StandardLateFeePolicy());
        Assert.Null(library.FindByIsbn("does-not-exist"));
    }

    [Fact]
    public void Borrow_Success_MarksBookUnavailableAndTracksMember()
    {
        var library = new Library(new StandardLateFeePolicy());
        library.AddBook(SampleBook());
        var member = new Member("M1", "Sara");

        library.Borrow(member, "111");

        Assert.False(library.FindByIsbn("111")!.IsAvailable);
        Assert.Contains("111", member.BorrowedIsbns);
    }

    [Fact]
    public void Borrow_UnknownIsbn_ThrowsArgumentException()
    {
        var library = new Library(new StandardLateFeePolicy());
        var member = new Member("M1", "Sara");
        Assert.Throws<ArgumentException>(() => library.Borrow(member, "nope"));
    }

    [Fact]
    public void Borrow_AlreadyBorrowed_ThrowsBookNotAvailable()
    {
        var library = new Library(new StandardLateFeePolicy());
        library.AddBook(SampleBook());
        library.Borrow(new Member("M1", "Sara"), "111");

        var member2 = new Member("M2", "Omar");
        Assert.Throws<BookNotAvailableException>(() => library.Borrow(member2, "111"));
    }

    [Fact]
    public void Borrow_AtLimit_ThrowsBorrowLimitExceeded()
    {
        var library = new Library(new StandardLateFeePolicy());
        library.AddBook(SampleBook("1"));
        library.AddBook(SampleBook("2"));
        library.AddBook(SampleBook("3"));
        library.AddBook(SampleBook("4"));
        var member = new Member("M1", "Sara");
        library.Borrow(member, "1");
        library.Borrow(member, "2");
        library.Borrow(member, "3");

        Assert.Throws<BorrowLimitExceededException>(() => library.Borrow(member, "4"));
    }

    [Fact]
    public void Return_MakesBookAvailableAndRemovesFromMember()
    {
        var library = new Library(new NoLateFeePolicy());
        library.AddBook(SampleBook());
        var member = new Member("M1", "Sara");
        library.Borrow(member, "111");

        library.Return(member, "111", 0);

        Assert.True(library.FindByIsbn("111")!.IsAvailable);
        Assert.Empty(member.BorrowedIsbns);
    }

    [Fact]
    public void Return_UsesInjectedPolicy_Standard()
    {
        var library = new Library(new StandardLateFeePolicy());
        library.AddBook(SampleBook());
        var member = new Member("M1", "Sara");
        library.Borrow(member, "111");

        double fee = library.Return(member, "111", 6);

        Assert.Equal(3.0, fee);
    }

    [Fact]
    public void Return_UsesInjectedPolicy_NoFee()
    {
        var library = new Library(new NoLateFeePolicy());
        library.AddBook(SampleBook());
        var member = new Member("M1", "Sara");
        library.Borrow(member, "111");

        double fee = library.Return(member, "111", 100);

        Assert.Equal(0, fee);
    }
}
