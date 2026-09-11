# Module 6, Lesson 02 — Relationships

Real data isn't one flat table. A course has many students; a student has many enrollments; an order has many line items. This lesson covers modeling **one-to-many** relationships in EF Core.

## The classic example: one Classroom has many Students

```csharp
public class Classroom
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public List<Student> Students { get; set; } = [];    // NAVIGATION PROPERTY — "the students in this classroom"
}

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public int ClassroomId { get; set; }        // FOREIGN KEY — which classroom this student belongs to
    public Classroom? Classroom { get; set; }    // NAVIGATION PROPERTY — "the classroom this student is in"
}
```

| Piece | Meaning |
|---|---|
| `ClassroomId` | The **foreign key** — an `int` matching some `Classroom.Id`. This is the actual column that exists in the database's `Students` table. |
| `Classroom? Classroom` | A **navigation property** — lets you write `student.Classroom.Name` in C# instead of a manual lookup. EF Core figures out the relationship from the naming convention (`ClassroomId` + a `Classroom` navigation property) automatically — no extra configuration needed for the simple case. |
| `List<Student> Students` on `Classroom` | The "many" side — every student belonging to this classroom. |

This is called a **one-to-many** relationship: one `Classroom`, many `Student`s.

## Creating related data

```csharp
var classroom = new Classroom { Name = "Grade 10A" };
classroom.Students.Add(new Student { Name = "Ali" });
classroom.Students.Add(new Student { Name = "Sara" });

context.Classrooms.Add(classroom);
await context.SaveChangesAsync();
```

You don't have to manually set `ClassroomId` on each student — adding them to `classroom.Students` and saving the whole graph together is enough; EF Core works out the foreign keys and inserts everything in the right order.

## Reading related data — and the lazy-loading trap

```csharp
var classroom = await context.Classrooms.FirstAsync(c => c.Name == "Grade 10A");
Console.WriteLine(classroom.Students.Count);    // 0 !! — even though students exist in the database
```

By default, EF Core does **not** automatically load related data — `classroom.Students` is just empty unless you explicitly asked for it. This is different from some other ORMs that support automatic "lazy loading" (fetching related data on first access); EF Core's default is **explicit loading**, which is more predictable and avoids accidentally firing off hundreds of extra queries without realizing it (a notorious performance trap called "N+1 queries").

## `Include` — eager loading

```csharp
using Microsoft.EntityFrameworkCore;

var classroom = await context.Classrooms
    .Include(c => c.Students)              // "also load the Students navigation property"
    .FirstAsync(c => c.Name == "Grade 10A");

Console.WriteLine(classroom.Students.Count);   // 2 — now it's actually loaded
```

`Include(c => c.Students)` tells EF Core: "when you run this query, also fetch the related students in the same round-trip." Forgetting `Include` when you need related data is one of the most common EF Core mistakes — if a navigation property looks unexpectedly empty, this is the first thing to check.

## Querying from the "many" side

You don't always need `Include` — sometimes you query the related table directly, using the foreign key or navigation property in a `Where`:

```csharp
List<Student> studentsInClassroom = await context.Students
    .Where(s => s.ClassroomId == classroomId)
    .ToListAsync();

// or, equivalently, navigating the relationship in the query itself:
List<Student> sameThing = await context.Students
    .Where(s => s.Classroom!.Name == "Grade 10A")
    .ToListAsync();
```

Both compile down to a `JOIN` in the generated SQL — you write LINQ, EF Core handles the actual join.

## Deleting — cascade behavior

```csharp
context.Classrooms.Remove(classroom);
await context.SaveChangesAsync();
```

By convention, EF Core deletes a `Classroom`'s related `Student` rows too (**cascade delete**) when the relationship is required (`ClassroomId` is a non-nullable `int`, meaning a student can't exist without a classroom). If that's not what you want, the relationship needs to be configured differently — beyond this lesson's scope, but good to know the default exists and why.

## Summary
- A **foreign key** (`ClassroomId`) plus matching **navigation properties** (`Classroom` on `Student`, `List<Student> Students` on `Classroom`) model a one-to-many relationship.
- EF Core infers the relationship from naming convention — no extra setup for the simple case.
- Related data is **not** loaded automatically — use `.Include(x => x.Nav)` to eager-load it, or query the related `DbSet` directly with `Where`.
- Forgetting `Include` is the most common cause of "why is this navigation property empty?"
- Deleting a required "one" side cascades to delete its related "many" side rows by default.
