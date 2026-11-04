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
        return await _context.Books.Where(b => b.Price < max).OrderBy(b => b.Title).Select(b => b.Title).ToListAsync();
    }

    public async Task<List<Book>> SearchAsync(string text)
    {
        return await _context.Books.Where(b => b.Title.Contains(text)).ToListAsync();
    }

    public async Task<double> AveragePriceAsync()
    {
        if (!await _context.Books.AnyAsync()) return 0;
        return await _context.Books.AverageAsync(b => b.Price);
    }
}
