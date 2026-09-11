// Exercise 01 — OpenAPI-documented BooksApi
// See README.md.

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var app = builder.Build();
app.MapOpenApi();

string[] books = ["Dune", "1984", "Foundation"];

// TODO: GET /books -> books, .WithName("GetAllBooks").WithSummary("Get every book")

// TODO: GET /books/{id:int} -> books[id] if in range, else Results.NotFound()
//       .WithName("GetBookById").WithSummary("Get a single book by index")

app.Run();

public partial class Program { }
