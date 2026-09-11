// Exercise 01 — StudentsApi with contract tests (solution)

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var students = new List<Student>
{
    new Student(1, "Ali", 85),
    new Student(2, "Sara", 92),
};

app.MapGet("/api/students", () => students);

app.MapGet("/api/students/{id:int}", (int id) =>
{
    var student = students.FirstOrDefault(s => s.Id == id);
    return student is not null ? Results.Ok(student) : Results.NotFound();
});

app.Run();

public record Student(int Id, string Name, int Grade);

public partial class Program { }
