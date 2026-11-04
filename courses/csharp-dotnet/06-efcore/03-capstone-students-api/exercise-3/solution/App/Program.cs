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

app.MapPut("/notes/{id:int}", async (AppDbContext db, int id, NewNote body) =>
{
    var note = await db.Notes.FindAsync(id);
    if (note is null) return Results.NotFound();
    note.Text = body.Text;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/notes/{id:int}", async (AppDbContext db, int id) =>
{
    var note = await db.Notes.FindAsync(id);
    if (note is null) return Results.NotFound();
    db.Notes.Remove(note);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/notes", async (AppDbContext db, int page = 1, int size = 10) =>
    await db.Notes.OrderBy(n => n.Id).Skip((page - 1) * size).Take(size).ToListAsync());

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
