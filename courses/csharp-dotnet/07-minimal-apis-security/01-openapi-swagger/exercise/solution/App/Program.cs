// Exercise 01 — OpenAPI-documented BooksApi (solution)

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var app = builder.Build();
app.MapOpenApi();

string[] books = ["Dune", "1984", "Foundation"];

app.MapGet("/books", () => books)
    .WithName("GetAllBooks")
    .WithSummary("Get every book");

app.MapGet("/books/{id:int}", (int id) =>
{
    return id >= 0 && id < books.Length ? Results.Ok(books[id]) : Results.NotFound();
})
    .WithName("GetBookById")
    .WithSummary("Get a single book by index");

app.Run();

public partial class Program { }
