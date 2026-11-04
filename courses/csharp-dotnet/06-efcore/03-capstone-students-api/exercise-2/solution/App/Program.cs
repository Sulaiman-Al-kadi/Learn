using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connection = new SqliteConnection("DataSource=:memory:");
connection.Open();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connection));
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}

app.MapPost("/notes", async (AppDbContext db, NewNote body) =>
{
    var note = new Note { Text = body.Text };
    db.Notes.Add(note);
    await db.SaveChangesAsync();
    return Results.Created($"/notes/{note.Id}", note);
});

app.MapGet("/notes", async (AppDbContext db) => await db.Notes.OrderByDescending(n => n.Id).ToListAsync());

app.MapGet("/notes/{id:int}", async (AppDbContext db, int id) =>
    await db.Notes.FindAsync(id) is Note note ? Results.Ok(note) : Results.NotFound());

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
