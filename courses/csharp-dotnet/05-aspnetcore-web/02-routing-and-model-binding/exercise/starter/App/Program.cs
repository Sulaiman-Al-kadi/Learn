// Exercise 02 — TodoApi routing & binding
// See README.md.

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// TODO: seed `todos` (List<TodoItem>) with 3 items, and `int nextId = 4;`

// TODO: GET /todos -> the full list

// TODO: GET /todos/{id:int} -> matching item or Results.NotFound()

// TODO: POST /todos (CreateTodoRequest body) -> validate Title, add, Results.Created(...)

// TODO: GET /todos/search (string? title query param) -> filtered list, or full list if title is empty

app.Run();

public record TodoItem(int Id, string Title, bool Done);
public record CreateTodoRequest(string Title);

public partial class Program { }
