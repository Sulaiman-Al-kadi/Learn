# Exercise — TodoApi routing & binding

Write the whole app in `App/Program.cs`.

## Types (put these at the bottom of the file, after `app.Run();`, same as the lesson example)
```csharp
public record TodoItem(int Id, string Title, bool Done);
public record CreateTodoRequest(string Title);
```

## Seed data
Before any endpoints, create the in-memory storage:
```csharp
var todos = new List<TodoItem>
{
    new TodoItem(1, "Buy milk", false),
    new TodoItem(2, "Write report", true),
    new TodoItem(3, "Walk dog", false),
};
int nextId = 4;
```

## Endpoints

| Route | Method | Behavior |
|---|---|---|
| `/todos` | GET | return the full `todos` list |
| `/todos/{id:int}` | GET | return the matching item, or `Results.NotFound()` if no item has that `Id` |
| `/todos` | POST | body is a `CreateTodoRequest`. If `Title` is null/whitespace, `Results.BadRequest("Title is required")`. Otherwise create a new `TodoItem` (`Id = nextId++`, `Done = false`), add it to `todos`, return `Results.Created($"/todos/{item.Id}", item)` |
| `/todos/search` | GET | query parameter `title` (a `string?`). Return every todo whose `Title` **contains** `title`, case-insensitive. If `title` is null/empty, return the full list |

## Rules
- `/todos/{id:int}` uses the explicit `:int` route constraint (lesson content).
- Use LINQ (Module 3) for the lookups — `FirstOrDefault` for the by-id search, `Where` for the title search.
- `search`'s case-insensitive check: `t.Title.Contains(title, StringComparison.OrdinalIgnoreCase)`.
