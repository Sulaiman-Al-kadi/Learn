var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var app = builder.Build();
app.MapOpenApi();

// TODO: see README.md

app.Run();

public record Book(int Id, string Title);

public partial class Program { }
