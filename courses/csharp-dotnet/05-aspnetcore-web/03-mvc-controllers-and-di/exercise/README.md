# Capstone — StudentsApi (MVC + DI)

`App/Program.cs` is already written (fixed infrastructure — DI registration and `app.MapControllers()`). Write everything else in `App/StudentsApi.cs`.

## `Student` (record)
```csharp
public record Student(int Id, string Name, int Grade);
```

## `IStudentRepository` (interface)
```csharp
public interface IStudentRepository
{
    List<Student> GetAll();
    Student? GetById(int id);
    Student Add(string name, int grade);
}
```

## `InMemoryStudentRepository : IStudentRepository`
- Seed it with two students: `(1, "Ali", 85)` and `(2, "Sara", 92)`.
- Track the next id starting at `3`.
- `GetAll()` returns the full list. `GetById(id)` returns the match or `null`. `Add(name, grade)` creates a new `Student` with the next id, adds it, and returns it.

## `CreateStudentRequest` (record)
```csharp
public record CreateStudentRequest(string Name, int Grade);
```

## `StudentsController : ControllerBase`
- `[ApiController]`, `[Route("api/[controller]")]` on the class.
- Constructor takes `IStudentRepository` (constructor injection — don't `new` the repository yourself; `Program.cs` already registers it with the DI container as a singleton).
- `[HttpGet]` → `GetAll()` → `Ok(_repository.GetAll())`.
- `[HttpGet("{id:int}")]` → `GetById(int id)` → the student, or `NotFound()`.
- `[HttpPost]` → `Create([FromBody] CreateStudentRequest request)`:
  - if `request.Name` is null/whitespace, `BadRequest("Name is required")`.
  - otherwise, add the student and return `CreatedAtAction(nameof(GetById), new { id = student.Id }, student)` — this produces a 201 response whose `Location` header points at the new student's `GetById` URL (a nicety `Results.Created` also offers, expressed the MVC-controller way).

## Rules
- `InMemoryStudentRepository` must be the **only** place that owns the actual list — `StudentsController` only ever talks to it through the `IStudentRepository` interface (DIP, exactly like the lesson).
- Because the repository is registered as a **singleton** in `Program.cs`, its state persists across requests within the same running app — that's what lets a `POST` followed by a `GET` see the new student.
