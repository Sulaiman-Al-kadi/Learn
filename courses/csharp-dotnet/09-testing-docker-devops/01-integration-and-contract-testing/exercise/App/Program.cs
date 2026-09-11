// Exercise 01 — StudentsApi with contract tests
// See README.md.

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var students = new List<Student>
{
    new Student(1, "Ali", 85),
    new Student(2, "Sara", 92),
};

// TODO: GET /api/students -> students

// TODO: GET /api/students/{id:int} -> matching student or Results.NotFound()

app.Run();

public record Student(int Id, string Name, int Grade);

public partial class Program { }
