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
        throw new NotImplementedException();
    }

    public async Task<Author?> GetWithBooksAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Dictionary<string, int>> BookCountsAsync()
    {
        throw new NotImplementedException();
    }
}
