# Module 5, Lesson 02 — Routing & Model Binding

Lesson 01's `/echo/{text}` briefly used a route parameter. This lesson covers the full picture: where a handler's parameters can come from, and how ASP.NET Core decides.

## Route parameters, precisely

```csharp
app.MapGet("/students/{id}", (int id) => $"Student #{id}");
```

`{id}` in the route template is a placeholder; the handler's `int id` parameter is matched to it **by name**. ASP.NET Core automatically converts the URL segment's text into an `int` — visiting `/students/abc` (not a valid int) means the route simply **doesn't match at all**, resulting in a 404, before your code ever runs.

## Route constraints — being explicit about the type

```csharp
app.MapGet("/students/{id:int}", (int id) => $"Student #{id}");
app.MapGet("/students/{name:alpha}", (string name) => $"Named {name}");
```

`{id:int}` explicitly requires the segment to parse as an integer (this is largely equivalent to the implicit inference above, but makes intent unambiguous and lets you catch mismatches at routing time rather than deep in your handler). Other constraints exist (`:guid`, `:alpha`, `:minlength(3)`, `:range(1,100)`) — you won't need most of them yet, but recognize the `{name:constraint}` syntax.

## Query strings — automatic for simple types

```csharp
app.MapGet("/search", (string? query, int page) => $"Searching '{query}', page {page}");
```

Visiting `/search?query=cats&page=2` binds `query` to `"cats"` and `page` to `2` **automatically** — no attribute needed. The rule: parameters of simple types (`string`, `int`, `bool`, `Guid`, ...) that **don't** match a route placeholder name are bound from the query string by default.

```csharp
app.MapGet("/search", (string? query, int page = 1) => $"Searching '{query}', page {page}");
```
A default value (`page = 1`) makes that query parameter optional — omit it in the URL and you get the default instead of an error.

## The request body — for complex types

```csharp
public record CreateStudentRequest(string Name, int Age);

app.MapPost("/students", (CreateStudentRequest request) => $"Creating {request.Name}, age {request.Age}");
```

A parameter whose type is a **class or record** (not a simple built-in type) is, by default, bound from the **JSON request body** — the client sends:
```json
{ "Name": "Sara", "Age": 25 }
```
and ASP.NET Core deserializes it into a `CreateStudentRequest` automatically, the same `System.Text.Json` machinery from Module 4, lesson 02, just wired into the pipeline for you.

## The binding rule, summarized

| Parameter shape | Bound from |
|---|---|
| Simple type, name matches a `{placeholder}` in the route | Route |
| Simple type, name does **not** match a route placeholder | Query string |
| Class/record type | Request body (JSON) |

You rarely need to think about this consciously once it clicks — it's designed so the "obvious" parameter list does the right thing.

## Validating input

Minimal APIs don't auto-validate for you (that's mostly an MVC-controller feature, lesson 03) — you check and respond explicitly:

```csharp
app.MapPost("/students", (CreateStudentRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.BadRequest("Name is required");
    }
    if (request.Age < 0)
    {
        return Results.BadRequest("Age cannot be negative");
    }

    // ... create the student ...
    return Results.Created($"/students/{newId}", createdStudent);
});
```

This is the same guard-clause pattern from Module 2, lesson 07 — just at the edge of your application instead of inside a class.

## Status codes that matter here

| `Results.` | HTTP status | When |
|---|---|---|
| `Ok(value)` | 200 | Successful `GET`, or a successful action returning data |
| `Created(location, value)` | 201 | Successfully created something new (typical `POST` response) |
| `NoContent()` | 204 | Success, nothing to return (typical successful `DELETE`) |
| `BadRequest(errors)` | 400 | The request itself was invalid (bad input) |
| `NotFound()` | 404 | The requested resource doesn't exist |

Returning the right status code isn't cosmetic — API clients (including your own frontend, or other services) branch their behavior on it.

## A complete small example

```csharp
var todos = new List<TodoItem> { new(1, "Buy milk", false) };
int nextId = 2;

app.MapGet("/todos", () => todos);

app.MapGet("/todos/{id:int}", (int id) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);     // Module 3!
    return todo is not null ? Results.Ok(todo) : Results.NotFound();
});

app.MapPost("/todos", (CreateTodoRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Title)) return Results.BadRequest("Title is required");
    var todo = new TodoItem(nextId++, request.Title, false);
    todos.Add(todo);
    return Results.Created($"/todos/{todo.Id}", todo);
});

public record TodoItem(int Id, string Title, bool Done);
public record CreateTodoRequest(string Title);
```

Every piece here is something you already know — records (Module 2), `FirstOrDefault` (Module 3), guard clauses (Module 2) — just arranged as HTTP endpoints instead of plain methods.

## Summary
- Route parameters: `{name}` in the path, matched to a same-named simple-type handler parameter. `{id:int}` makes the type constraint explicit.
- Query string parameters: simple types whose names **don't** match a route placeholder; `= defaultValue` makes them optional.
- Request body: class/record-typed parameters are deserialized from JSON automatically.
- Validate manually with guard clauses, returning `Results.BadRequest(...)` for invalid input.
- `Results.Ok/Created/NoContent/BadRequest/NotFound` map directly to the HTTP status codes API clients rely on.
