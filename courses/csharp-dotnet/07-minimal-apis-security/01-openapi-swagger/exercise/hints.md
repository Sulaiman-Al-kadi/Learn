## Hint 1
```csharp
app.MapGet("/books", () => books)
    .WithName("GetAllBooks")
    .WithSummary("Get every book");
```

## Hint 2
```csharp
app.MapGet("/books/{id:int}", (int id) =>
{
    return id >= 0 && id < books.Length ? Results.Ok(books[id]) : Results.NotFound();
})
    .WithName("GetBookById")
    .WithSummary("Get a single book by index");
```

## Hint 3
`.WithName(...)` and `.WithSummary(...)` chain directly onto the result of `MapGet(...)` — no semicolon until after the last one in the chain.
