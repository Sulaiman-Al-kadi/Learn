using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class ShopServiceTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly AppDbContext _context;
    private readonly ShopService _service;

    public ShopServiceTests()
    {
        _connection.Open();
        _context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options);
        _context.Database.EnsureCreated();
        _service = new ShopService(_context);
    }

    public void Dispose() { _context.Dispose(); _connection.Close(); }

    private async Task<int> SeedAsync()
    {
        var sara = new Customer { Name = "Sara", Orders = [new Order { Total = 60 }, new Order { Total = 50 }] };
        _context.Customers.AddRange(sara, new Customer { Name = "Ali", Orders = [new Order { Total = 200 }] }, new Customer { Name = "Omar" });
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
        return sara.Id;
    }

    [Fact] public async Task Delete_ReturnsOrderCount() => Assert.Equal(2, await _service.DeleteCustomerAsync(await SeedAsync()));
    [Fact] public async Task Delete_Cascades()
    {
        await _service.DeleteCustomerAsync(await SeedAsync());
        Assert.Equal(1, await _context.Orders.CountAsync());
    }
    [Fact] public async Task Delete_Missing() => Assert.Equal(-1, await _service.DeleteCustomerAsync(99));
    [Fact] public async Task BigSpenders_UsesTotal()
    {
        await SeedAsync();
        Assert.Equal(["Ali", "Sara"], await _service.BigSpendersAsync(100));
    }
}
