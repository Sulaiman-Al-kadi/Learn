## Hint 1
Seed data and endpoints all go between `var app = builder.Build();` and `app.Run();`, exactly like the lesson's example:
```csharp
var todos = new List<TodoItem>
{
    new TodoItem(1, "Buy milk", false),
    new TodoItem(2, "Write report", true),
    new TodoItem(3, "Walk dog", false),
};
int nextId = 4;
```

## Hint 2
```csharp
app.MapGet("/todos", () => todos);

app.MapGet("/todos/{id:int}", (int id) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    return todo is not null ? Results.Ok(todo) : Results.NotFound();
});
```

## Hint 3
```csharp
app.MapPost("/todos", (CreateTodoRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Title)) return Results.BadRequest("Title is required");
    var todo = new TodoItem(nextId++, request.Title, false);
    todos.Add(todo);
    return Results.Created($"/todos/{todo.Id}", todo);
});
```

## Hint 4
```csharp
app.MapGet("/todos/search", (string? title) =>
{
    if (string.IsNullOrEmpty(title)) return todos;
    return todos.Where(t => t.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
});
```
