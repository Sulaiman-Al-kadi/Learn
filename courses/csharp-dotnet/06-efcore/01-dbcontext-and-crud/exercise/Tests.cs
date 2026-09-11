using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

// Each test opens its own in-memory SQLite connection (kept open for the test's duration —
// closing it would wipe an in-memory database) so tests never share state.
public class StudentServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly StudentService _service;

    public StudentServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
        _service = new StudentService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
    }

    [Fact]
    public async Task AddAsync_PersistsAndAssignsId()
    {
        var student = await _service.AddAsync("Ali", 85);

        Assert.True(student.Id > 0);
        Assert.Equal("Ali", student.Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEveryAddedStudent()
    {
        await _service.AddAsync("Ali", 85);
        await _service.AddAsync("Sara", 92);

        var all = await _service.GetAllAsync();

        Assert.Equal(2, all.Count);
    }

    [Fact]
    public async Task GetByIdAsync_Found_ReturnsStudent()
    {
        var added = await _service.AddAsync("Omar", 70);

        var found = await _service.GetByIdAsync(added.Id);

        Assert.NotNull(found);
        Assert.Equal("Omar", found!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsNull()
    {
        var found = await _service.GetByIdAsync(999);
        Assert.Null(found);
    }

    [Fact]
    public async Task UpdateGradeAsync_Found_UpdatesAndReturnsTrue()
    {
        var added = await _service.AddAsync("Lina", 60);

        bool result = await _service.UpdateGradeAsync(added.Id, 95);

        Assert.True(result);
        var updated = await _service.GetByIdAsync(added.Id);
        Assert.Equal(95, updated!.Grade);
    }

    [Fact]
    public async Task UpdateGradeAsync_NotFound_ReturnsFalse()
    {
        bool result = await _service.UpdateGradeAsync(999, 95);
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_Found_RemovesAndReturnsTrue()
    {
        var added = await _service.AddAsync("Zaid", 80);

        bool result = await _service.DeleteAsync(added.Id);

        Assert.True(result);
        Assert.Null(await _service.GetByIdAsync(added.Id));
    }

    [Fact]
    public async Task DeleteAsync_NotFound_ReturnsFalse()
    {
        bool result = await _service.DeleteAsync(999);
        Assert.False(result);
    }
}
