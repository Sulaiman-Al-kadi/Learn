using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class LibraryServiceTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly AppDbContext _context;
    private readonly LibraryService _service;

    public LibraryServiceTests()
    {
        _connection.Open();
        _context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options);
        _context.Database.EnsureCreated();
        _service = new LibraryService(_context);
    }

    public void Dispose() { _context.Dispose(); _connection.Close(); }

    private async Task<int> AuthorAsync(string name)
    {
        var author = new Author { Name = name };
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        return author.Id;
    }

    [Fact] public async Task AddBook_MissingAuthor_False() => Assert.False(await _service.AddBookAsync(99, "x"));
    [Fact] public async Task GetWithBooks_LoadsBooks()
    {
        int id = await AuthorAsync("Austen");
        await _service.AddBookAsync(id, "Emma");
        _context.ChangeTracker.Clear();
        Assert.Equal("Emma", (await _service.GetWithBooksAsync(id))!.Books.Single().Title);
    }
    [Fact] public async Task GetWithBooks_Missing_Null() => Assert.Null(await _service.GetWithBooksAsync(99));
    [Fact] public async Task Counts_IncludeZero()
    {
        int a = await AuthorAsync("Austen");
        await AuthorAsync("Nobody");
        await _service.AddBookAsync(a, "Emma");
        await _service.AddBookAsync(a, "Persuasion");
        _context.ChangeTracker.Clear();
        var counts = await _service.BookCountsAsync();
        Assert.Equal(2, counts["Austen"]);
        Assert.Equal(0, counts["Nobody"]);
    }
}
