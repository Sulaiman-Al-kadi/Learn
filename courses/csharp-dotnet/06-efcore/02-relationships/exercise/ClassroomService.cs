// Exercise 02 — Classroom/Student relationship
// See README.md.
using Microsoft.EntityFrameworkCore;

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

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Classroom> Classrooms => Set<Classroom>();
    public DbSet<Student> Students => Set<Student>();
}

public class ClassroomService
{
    // TODO: private readonly AppDbContext field, set via constructor
    public ClassroomService(AppDbContext context)
    {
        throw new NotImplementedException();
    }

    public async Task<Classroom> CreateClassroomAsync(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<Student> AddStudentAsync(int classroomId, string name)
    {
        throw new NotImplementedException();
    }

    public async Task<Classroom?> GetClassroomWithStudentsAsync(int classroomId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Student>> GetStudentsInClassroomAsync(int classroomId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteClassroomAsync(int classroomId)
    {
        throw new NotImplementedException();
    }
}
