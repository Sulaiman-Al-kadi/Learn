# Exercise — StudentsApi with contract tests

Write the whole app in `App/Program.cs`.

## Seed data
```csharp
var students = new List<Student>
{
    new Student(1, "Ali", 85),
    new Student(2, "Sara", 92),
};
```
```csharp
public record Student(int Id, string Name, int Grade);
```

## Endpoints
| Route | Method | Behavior |
|---|---|---|
| `/api/students` | GET | the full list |
| `/api/students/{id:int}` | GET | the matching student, or `Results.NotFound()` |

## How it's tested
`Tests.cs` has **two kinds** of tests over the same endpoints — this is the lesson's point, made concrete:
- **Integration tests** check actual values (`"Ali"`, `85`, ...).
- **Contract tests** check the response **shape** using `JsonDocument` — that `id`/`name`/`grade` exist with the right JSON types, regardless of the actual values.

Both kinds must pass, which they will automatically once your endpoints return the right data — you don't write anything differently for the contract tests, they're just a different lens on the same responses.
