using Microsoft.EntityFrameworkCore;

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<Book> Books { get; set; } = [];
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
}

public class LibraryService
{
    private readonly AppDbContext _context;

    public LibraryService(AppDbContext context) => _context = context;

    public async Task<bool> AddBookAsync(int authorId, string title)
    {
        if (!await _context.Authors.AnyAsync(a => a.Id == authorId)) return false;
        _context.Books.Add(new Book { Title = title, AuthorId = authorId });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Author?> GetWithBooksAsync(int id)
    {
        return await _context.Authors.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Dictionary<string, int>> BookCountsAsync()
    {
        return await _context.Authors
            .Select(a => new { a.Name, Count = a.Books.Count })
            .ToDictionaryAsync(x => x.Name, x => x.Count);
    }
}
