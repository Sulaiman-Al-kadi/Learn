var builder = WebApplication.CreateBuilder(args);
// TODO: see README.md

var app = builder.Build();
// TODO: see README.md

app.MapGet("/data", () => "data");

app.Run();

public partial class Program { }
