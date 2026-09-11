# Exercise — StudentService with EF Core

Write everything in `StudentService.cs`.

## `Student` (entity)
```csharp
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Grade { get; set; }
}
```

## `AppDbContext`
```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Student> Students => Set<Student>();
}
```

## `StudentService`
Constructor takes an `AppDbContext` (injected, don't create one inside).

| Method | Does |
|---|---|
| `Task<Student> AddAsync(string name, int grade)` | create a `Student`, add it, `SaveChangesAsync`, return it (its `Id` will be set after saving) |
| `Task<List<Student>> GetAllAsync()` | every student |
| `Task<Student?> GetByIdAsync(int id)` | the student, or `null` |
| `Task<bool> UpdateGradeAsync(int id, int newGrade)` | find the student; if not found, return `false`. Otherwise update its `Grade`, save, return `true` |
| `Task<bool> DeleteAsync(int id)` | find the student; if not found, return `false`. Otherwise remove it, save, return `true` |

## Rules
- Every method is `async` and uses the `...Async` EF Core methods (`ToListAsync`, `FindAsync`, `SaveChangesAsync`) — never the synchronous versions.
- Use `_context.Students.FindAsync(id)` to look a student up by primary key — it's the standard, most efficient way (faster than `FirstOrDefaultAsync(s => s.Id == id)` for a primary-key lookup, though both would work).
