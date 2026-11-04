using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connection = new SqliteConnection("DataSource=:memory:");
connection.Open();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connection));
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    db.Notes.AddRange(new Note { Text = "a" }, new Note { Text = "b" }, new Note { Text = "c" });
    db.SaveChanges();
}

// TODO: see README.md

app.Run();

public class Note
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
}

public record NewNote(string Text);

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Note> Notes => Set<Note>();
}

public partial class Program { }
