var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var app = builder.Build();
app.MapOpenApi();

app.MapGet("/books/{id:int}", (int id) => id == 1 ? Results.Ok(new Book(1, "Dune")) : Results.NotFound())
    .Produces<Book>(200)
    .Produces(404);

app.Run();

public record Book(int Id, string Title);

public partial class Program { }
