var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// TODO: see README.md

app.MapGet("/public", () => "hello");
app.MapGet("/admin/stats", () => "stats");

app.Run();

public partial class Program { }
