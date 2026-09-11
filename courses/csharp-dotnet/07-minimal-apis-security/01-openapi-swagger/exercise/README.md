# Exercise — OpenAPI-documented BooksApi

Write the whole app in `App/Program.cs`.

## Setup
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var app = builder.Build();
app.MapOpenApi();
```

## Endpoints

| Route | Method | Returns | Annotations |
|---|---|---|---|
| `/books` | GET | `new[] { "Dune", "1984", "Foundation" }` | `.WithName("GetAllBooks")`, `.WithSummary("Get every book")` |
| `/books/{id:int}` | GET | the matching title from the array above (`0` → `"Dune"`, `1` → `"1984"`, `2` → `"Foundation"`), or `Results.NotFound()` for any other index | `.WithName("GetBookById")`, `.WithSummary("Get a single book by index")` |

## Rules
- `AddOpenApi()`/`MapOpenApi()` must be present so `/openapi/v1.json` is reachable.
- Both endpoints need `.WithName(...)` and `.WithSummary(...)` chained onto their `MapGet(...)` call.
- `/books/{id:int}` must return `Results.NotFound()` for an out-of-range index rather than throwing.
