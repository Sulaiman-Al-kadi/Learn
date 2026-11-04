var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var books = new List<Book> { new(1, "Dune", "Herbert"), new(2, "Emma", "Austen"), new(3, "Persuasion", "Austen") };

app.MapGet("/books/{id:int}", (int id) =>
{
    var book = books.FirstOrDefault(b => b.Id == id);
    return book is not null ? Results.Ok(book) : Results.NotFound();
});

app.MapGet("/books", (string? author) =>
    author is null ? books : books.Where(b => b.Author.Equals(author, StringComparison.OrdinalIgnoreCase)).ToList());

app.Run();

public record Book(int Id, string Title, string Author);

public partial class Program { }
