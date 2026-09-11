## Hint 1
The middleware goes right after `var app = builder.Build();` and before any `app.MapGet(...)`:
```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Powered-By", "LearnLab");
    await next();
});
```

## Hint 2
`app.MapGet("/hello", () => "Hello, World!");` — a handler with no parameters, returning a plain string.

## Hint 3
`app.MapGet("/students", () => new[] { "Ali", "Sara", "Omar" });` — returning an array automatically becomes a JSON response.

## Hint 4
`app.MapGet("/echo/{text}", (string text) => text);` — the parameter name `text` matches the route placeholder `{text}`, so ASP.NET Core fills it in from the URL automatically.
