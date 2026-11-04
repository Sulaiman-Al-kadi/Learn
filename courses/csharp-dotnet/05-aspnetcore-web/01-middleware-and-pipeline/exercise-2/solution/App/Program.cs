var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

int count = 0;
app.Use(async (context, next) =>
{
    count++;
    context.Response.Headers["X-Request-Count"] = count.ToString();
    await next(context);
});

app.MapGet("/ping", () => "pong");

app.Run();

public partial class Program { }
