# Module 6, Lesson 01 — DbContext & CRUD

Every exercise so far that "stored" data used a `List<T>` in memory — gone the moment the program stops. A real application needs a **database**. **Entity Framework Core (EF Core)** is .NET's tool for working with a database using ordinary C# objects and LINQ, instead of writing raw SQL by hand.

## The core idea: an "entity" is a plain class

```csharp
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Grade { get; set; }
}
```

Nothing database-specific here — it's a class exactly like every other one this course has written. EF Core's job is mapping objects like this to rows in a database table, and back.

## `DbContext` — your gateway to the database

```csharp
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
}
```

| Piece | Meaning |
|---|---|
| `: DbContext` | Inheritance (Module 2, lesson 03) — your context IS-A `DbContext`, gaining all its database machinery. |
| `DbContextOptions<AppDbContext> options` | Configuration (which database, connection string) — supplied from outside, another constructor injection (Module 2, lesson 08!). |
| `DbSet<Student> Students` | Represents the `Students` **table**. Querying it with LINQ (Module 3!) queries the table; adding to it stages an insert. |

## Configuring and registering the context

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));
```

This is Module 5's DI container again — `AppDbContext` becomes injectable anywhere, the same way `IStudentRepository` was. `UseSqlite(...)` says which database engine and where; SQLite stores the whole database in a single file (`app.db`) — no separate database server needed, perfect for learning and small projects. (Later, professional projects often use SQL Server or PostgreSQL — same EF Core code, different `Use...` call.)

## CRUD — Create, Read, Update, Delete

```csharp
// CREATE
var student = new Student { Name = "Ali", Grade = 85 };
context.Students.Add(student);
await context.SaveChangesAsync();          // nothing actually hits the database until SaveChanges!
Console.WriteLine(student.Id);              // the database generated this — available after SaveChanges

// READ
List<Student> all = await context.Students.ToListAsync();                    // Module 3's LINQ, now against a database
Student? one = await context.Students.FirstOrDefaultAsync(s => s.Id == 1);    // same LINQ methods you already know

// UPDATE
Student? toUpdate = await context.Students.FindAsync(1);
if (toUpdate is not null)
{
    toUpdate.Grade = 90;
    await context.SaveChangesAsync();        // EF Core tracks the change and updates just that row
}

// DELETE
Student? toDelete = await context.Students.FindAsync(1);
if (toDelete is not null)
{
    context.Students.Remove(toDelete);
    await context.SaveChangesAsync();
}
```

Notice: **every LINQ method you already know** (`ToListAsync`, `FirstOrDefaultAsync`, `Where`, ...) works here too — the `Async` versions exist because a database query is I/O (Module 4, lesson 01!) and shouldn't block the thread while waiting.

## The crucial idea: change tracking

EF Core watches objects it loaded (or you added) for changes:

```csharp
var student = await context.Students.FindAsync(1);   // EF Core now "tracks" this object
student.Grade = 95;                                     // just a normal property set — no special API
await context.SaveChangesAsync();                        // EF Core notices Grade changed, generates an UPDATE
```

You never write `UPDATE Students SET Grade = 95 WHERE Id = 1` yourself — EF Core compares the object's current state to what it originally loaded and generates the right SQL. This is genuinely the main value EF Core provides: you think in objects and LINQ; it thinks in SQL.

## Migrations — evolving the database schema (conceptually)

Your `Student` class defines the **shape** the database should have. A **migration** is a generated, versioned script that changes the actual database to match your classes:

```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

The first command inspects your `DbContext`/entity classes and writes a migration file describing "create a Students table with these columns." The second actually applies it to the real database file. Every time you change an entity (add a property, add a new entity), you add a new migration and apply it — this is how a database schema evolves alongside your code, with a full history, instead of being edited by hand.

For this lesson's exercise, you'll use `EnsureCreated()` instead (a shortcut that creates the schema directly from your entity classes, no migration history — fine for learning and for tests, but real projects use migrations so schema changes are tracked and repeatable across environments).

## Summary
- An **entity** is a plain class; `DbSet<T>` on your `DbContext` represents its table.
- `DbContext` is configured (`UseSqlite(...)`) and registered with DI (Module 5), just like any other service.
- CRUD: `Add` + `SaveChangesAsync` (create), LINQ queries (read), modify a tracked object + `SaveChangesAsync` (update), `Remove` + `SaveChangesAsync` (delete).
- EF Core's **change tracking** means you just mutate C# objects — it figures out the SQL.
- **Migrations** (`dotnet ef migrations add`, `dotnet ef database update`) evolve the real database schema to match your entity classes over time; `EnsureCreated()` is a simpler one-shot alternative used here for exercises/tests.
