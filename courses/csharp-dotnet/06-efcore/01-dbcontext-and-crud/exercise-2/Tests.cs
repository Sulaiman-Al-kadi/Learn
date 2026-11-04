using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class BookServiceTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly AppDbContext _context;
    private readonly BookService _service;

    public BookServiceTests()
    {
        _connection.Open();
        _context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options);
        _context.Database.EnsureCreated();
        _service = new BookService(_context);
    }

    public void Dispose() { _context.Dispose(); _connection.Close(); }

    [Fact] public async Task Add_AssignsId() => Assert.True((await _service.AddAsync("Dune", 10m)).Id > 0);
    [Fact] public async Task Update_Persists()
    {
        var book = await _service.AddAsync("Dune", 10m);
        Assert.True(await _service.UpdatePriceAsync(book.Id, 12m));
        _context.ChangeTracker.Clear();
        Assert.Equal(12m, (await _context.Books.SingleAsync()).Price);
    }
    [Fact] public async Task Update_Missing_False() => Assert.False(await _service.UpdatePriceAsync(99, 1m));
    [Fact] public async Task Delete_Removes()
    {
        var book = await _service.AddAsync("Dune", 10m);
        Assert.True(await _service.DeleteAsync(book.Id));
        Assert.Equal(0, await _context.Books.CountAsync());
    }
    [Fact] public async Task Delete_Missing_False() => Assert.False(await _service.DeleteAsync(99));
}
