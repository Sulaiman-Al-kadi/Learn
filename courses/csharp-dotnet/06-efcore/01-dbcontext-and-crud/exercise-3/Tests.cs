using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class BookQueryTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly AppDbContext _context;
    private readonly BookQueries _service;

    public BookQueryTests()
    {
        _connection.Open();
        _context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options);
        _context.Database.EnsureCreated();
        _service = new BookQueries(_context);
    }

    public void Dispose() { _context.Dispose(); _connection.Close(); }

    private async Task SeedAsync()
    {
        _context.Books.AddRange(new Book { Title = "Zen", Price = 5 }, new Book { Title = "Art", Price = 7 }, new Book { Title = "Art of War", Price = 30 });
        await _context.SaveChangesAsync();
    }

    [Fact] public async Task Cheaper_Sorted()
    {
        await SeedAsync();
        Assert.Equal(["Art", "Zen"], await _service.CheaperThanAsync(10));
    }
    [Fact] public async Task Search_Contains()
    {
        await SeedAsync();
        Assert.Equal(2, (await _service.SearchAsync("Art")).Count);
    }
    [Fact] public async Task Average_Basic()
    {
        await SeedAsync();
        Assert.Equal(14, await _service.AveragePriceAsync());
    }
    [Fact] public async Task Average_Empty() => Assert.Equal(0, await _service.AveragePriceAsync());
}
