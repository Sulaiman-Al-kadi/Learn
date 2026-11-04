var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

int count = 0;
// TODO: see README.md

app.MapGet("/ping", () => "pong");

app.Run();

public partial class Program { }
