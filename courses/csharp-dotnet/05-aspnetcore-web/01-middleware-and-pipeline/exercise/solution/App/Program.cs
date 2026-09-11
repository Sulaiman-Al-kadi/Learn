// Exercise 01 — Custom middleware + endpoints (solution)

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Powered-By", "LearnLab");
    await next();
});

app.MapGet("/hello", () => "Hello, World!");

app.MapGet("/students", () => new[] { "Ali", "Sara", "Omar" });

app.MapGet("/echo/{text}", (string text) => text);

app.Run();

public partial class Program { }
