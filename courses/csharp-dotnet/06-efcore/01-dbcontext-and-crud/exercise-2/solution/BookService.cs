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
        var book = new Book { Title = title, Price = price };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<bool> UpdatePriceAsync(int id, decimal price)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null) return false;
        book.Price = price;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null) return false;
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }
}
