# Module 6 Capstone — StudentsApi v2 (EF Core + SQLite)

Module 5 built a `StudentsApi` backed by an in-memory `List<Student>` — real, but everything vanished when the app restarted. This capstone rebuilds it on a real SQLite database with EF Core, matching the university plan's Week 4 deliverable exactly: "CRUD API connected to SQLite."

## What changes from Module 5's version

Almost nothing about `StudentsController` conceptually changes — it still depends on an abstraction and calls its methods. What changes is what's *behind* that abstraction:

```csharp
// Module 5: in-memory, gone on restart
public class InMemoryStudentRepository : IStudentRepository
{
    private readonly List<Student> _students = [...];
    public List<Student> GetAll() => _students;
    ...
}

// Module 6: a real database, durable
public class EfStudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;
    public EfStudentRepository(AppDbContext context) => _context = context;

    public async Task<List<Student>> GetAllAsync() => await _context.Students.ToListAsync();
    ...
}
```

This is the entire point of programming to an interface (Module 2, lesson 08; Module 5, lesson 03): the **controller never changes** when the storage mechanism underneath does. You could swap `EfStudentRepository` for a Redis-backed one, a mocked test double, or (as you just did) an in-memory one — the controller's code is unaffected either way, because it only ever talks to `IStudentRepository`.

## Registering the database in `Program.cs`

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=students.db"));
builder.Services.AddScoped<IStudentRepository, EfStudentRepository>();     // note: Scoped, not Singleton!
builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())        // create the database schema on startup
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

app.MapControllers();
app.Run();
```

Notice `AddScoped`, not `AddSingleton` (Module 5, lesson 03's lifetime table) — a `DbContext` is **not** safe to share across concurrent requests, so it's registered per-request instead. This is exactly the "anything touching a database connection" case that lesson mentioned.

## Testing it — the same `WebApplicationFactory` pattern, adapted

Real integration tests for a database-backed API typically swap in a **test-only database configuration** rather than hitting the real `students.db` file — otherwise tests would corrupt real data and interfere with each other. `WebApplicationFactory` supports this via `ConfigureTestServices`:

```csharp
public class TestingFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();          // remove the real SQLite-file config
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));   // swap in an in-memory one
        });
    }
}
```

You don't need to write this yourself for the exercise — it's provided in `Tests.cs` — but recognize the pattern: **remove** the production service registration, **add** a test-friendly replacement, exactly the substitution idea from Module 4 lesson 03's fake `HttpMessageHandler`, applied to a database instead of an HTTP call.

## Summary
- The exact same `IStudentRepository` interface and `StudentsController` from Module 5 now sit in front of a real EF Core + SQLite repository — nothing about the controller had to change.
- `DbContext` is registered `Scoped` (per request), never `Singleton`.
- `context.Database.EnsureCreated()` at startup creates the schema (a migration would be the real-world equivalent — lesson 01).
- Integration tests substitute an in-memory SQLite connection for the real database file, via `ConfigureWebHost` + `RemoveAll`/`AddDbContext` — the same "swap the real dependency for a test one" idea used throughout this course.

## The capstone

Open the exercise. You'll write `EfStudentRepository` and wire the whole thing together — the same CRUD behavior as Module 5's capstone, now backed by a real, durable SQLite database.
