using Microsoft.EntityFrameworkCore;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public double Price { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Book> Books => Set<Book>();
}

public class BookQueries
{
    private readonly AppDbContext _context;

    public BookQueries(AppDbContext context) => _context = context;

    public async Task<List<string>> CheaperThanAsync(double max)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Book>> SearchAsync(string text)
    {
        throw new NotImplementedException();
    }

    public async Task<double> AveragePriceAsync()
    {
        throw new NotImplementedException();
    }
}
