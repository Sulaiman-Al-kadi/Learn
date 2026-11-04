var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/add/{a:int}/{b:int}", (int a, int b) => a + b);

app.MapGet("/divide", (int a, int b) =>
    b == 0 ? Results.BadRequest("Cannot divide by zero") : Results.Ok(a / b));

app.MapPost("/sum", (int[] numbers) => numbers.Sum());

app.Run();

public partial class Program { }
