# Exercise — Classroom/Student relationship

Write everything in `ClassroomService.cs`.

## Entities
```csharp
public class Classroom
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<Student> Students { get; set; } = new List<Student>();
}

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int ClassroomId { get; set; }
    public Classroom? Classroom { get; set; }
}
```

## `AppDbContext`
```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Classroom> Classrooms => Set<Classroom>();
    public DbSet<Student> Students => Set<Student>();
}
```

## `ClassroomService`
Constructor takes an `AppDbContext`.

| Method | Does |
|---|---|
| `Task<Classroom> CreateClassroomAsync(string name)` | create and save a `Classroom`, return it |
| `Task<Student> AddStudentAsync(int classroomId, string name)` | create a `Student` with the given `ClassroomId`, save, return it |
| `Task<Classroom?> GetClassroomWithStudentsAsync(int classroomId)` | the classroom **with its `Students` eager-loaded** (use `Include`), or `null` |
| `Task<List<Student>> GetStudentsInClassroomAsync(int classroomId)` | every student whose `ClassroomId` matches, queried directly from `Students` |
| `Task<bool> DeleteClassroomAsync(int classroomId)` | find the classroom; if not found, `false`. Otherwise remove it (this cascades to its students — lesson content), save, return `true` |

## Rules
- `GetClassroomWithStudentsAsync` **must** use `.Include(c => c.Students)` — without it, the returned classroom's `Students` list would be empty even though matching rows exist, which is exactly the trap the lesson describes.
- `GetStudentsInClassroomAsync` queries `_context.Students` directly with `Where`, not through a loaded `Classroom`.
