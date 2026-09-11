## Hint 1
`app.MapGet("/api/students", () => students);`

## Hint 2
```csharp
app.MapGet("/api/students/{id:int}", (int id) =>
{
    var student = students.FirstOrDefault(s => s.Id == id);
    return student is not null ? Results.Ok(student) : Results.NotFound();
});
```

## Hint 3
The contract tests need no special handling from you — they inspect the SAME JSON your endpoints already return. If the integration tests pass, the contract tests will too, as long as your `Student` record's property names are `Id`, `Name`, `Grade` (JSON serializes them lowercase-first-letter by default: `id`, `name`, `grade`).
