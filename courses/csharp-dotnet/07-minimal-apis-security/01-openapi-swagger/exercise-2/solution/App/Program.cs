var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var app = builder.Build();
app.MapOpenApi();

app.MapGet("/books", () => new[] { "Dune" })
    .WithName("ListBooks")
    .WithSummary("List every book")
    .WithTags("Books");

app.Run();

public partial class Program { }
