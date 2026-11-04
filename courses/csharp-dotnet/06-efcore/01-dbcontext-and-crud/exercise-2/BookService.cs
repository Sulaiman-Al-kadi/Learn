using Microsoft.EntityFrameworkCore;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public decimal Price { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Book> Books => Set<Book>();
}

public class BookService
{
    private readonly AppDbContext _context;

    public BookService(AppDbContext context) => _context = context;

    public async Task<Book> AddAsync(string title, decimal price)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdatePriceAsync(int id, decimal price)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}
