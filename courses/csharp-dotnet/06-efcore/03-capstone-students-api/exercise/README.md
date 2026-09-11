# Capstone — StudentsApi v2 (EF Core + SQLite)

`App/Shared.cs` and `App/Program.cs` are already written (fixed infrastructure). Write `App/EfStudentRepository.cs`.

## `EfStudentRepository : IStudentRepository`

```csharp
public interface IStudentRepository
{
    Task<List<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(int id);
    Task<Student> AddAsync(string name, int grade);
}
```
(already declared in `Shared.cs` — implement it)

- Constructor takes an `AppDbContext` (injected — `Program.cs` registers everything for you).
- `GetAllAsync()` → every student, via `ToListAsync()`.
- `GetByIdAsync(id)` → the matching student or `null`, via `FindAsync`.
- `AddAsync(name, grade)` → create a `Student`, add it, `SaveChangesAsync`, return it.

This is almost identical to lesson 01's `StudentService` — same shape, now behind the `IStudentRepository` interface so `StudentsController` (in `Shared.cs`, already written) can use it without knowing anything database-specific.

## How it's tested

`Tests.cs` includes a `TestingFactory` (a `WebApplicationFactory<Program>` subclass) that swaps the real SQLite file for an in-memory SQLite connection during tests — see the lesson for exactly why and how. You don't need to modify it; just make `EfStudentRepository` correct and the whole API works, real database file or test in-memory one.
