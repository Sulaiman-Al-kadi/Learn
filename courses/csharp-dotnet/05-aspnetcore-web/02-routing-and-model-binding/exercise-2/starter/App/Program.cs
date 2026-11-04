var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var books = new List<Book> { new(1, "Dune", "Herbert"), new(2, "Emma", "Austen"), new(3, "Persuasion", "Austen") };

// TODO: see README.md

app.Run();

public record Book(int Id, string Title, string Author);

public partial class Program { }
