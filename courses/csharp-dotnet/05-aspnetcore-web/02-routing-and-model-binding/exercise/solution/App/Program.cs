// Exercise 02 — TodoApi routing & binding (solution)

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var todos = new List<TodoItem>
{
    new TodoItem(1, "Buy milk", false),
    new TodoItem(2, "Write report", true),
    new TodoItem(3, "Walk dog", false),
};
int nextId = 4;

app.MapGet("/todos", () => todos);

app.MapGet("/todos/{id:int}", (int id) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    return todo is not null ? Results.Ok(todo) : Results.NotFound();
});

app.MapPost("/todos", (CreateTodoRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Title)) return Results.BadRequest("Title is required");
    var todo = new TodoItem(nextId++, request.Title, false);
    todos.Add(todo);
    return Results.Created($"/todos/{todo.Id}", todo);
});

app.MapGet("/todos/search", (string? title) =>
{
    if (string.IsNullOrEmpty(title)) return todos;
    return todos.Where(t => t.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
});

app.Run();

public record TodoItem(int Id, string Title, bool Done);
public record CreateTodoRequest(string Title);

public partial class Program { }
