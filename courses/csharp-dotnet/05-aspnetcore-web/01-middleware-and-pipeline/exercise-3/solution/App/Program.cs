var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(async (context, next) =>
{
    bool isAdmin = context.Request.Path.StartsWithSegments("/admin");
    if (isAdmin && context.Request.Headers["X-Api-Key"] != "secret")
    {
        context.Response.StatusCode = 401;
        return;
    }
    await next(context);
});

app.MapGet("/public", () => "hello");
app.MapGet("/admin/stats", () => "stats");

app.Run();

public partial class Program { }
