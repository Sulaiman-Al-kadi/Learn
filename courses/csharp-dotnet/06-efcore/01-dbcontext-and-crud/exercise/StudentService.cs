// Exercise 01 — StudentService with EF Core
// See README.md.
using Microsoft.EntityFrameworkCore;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Grade { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Student> Students => Set<Student>();
}

public class StudentService
{
    // TODO: private readonly AppDbContext field, set via constructor
    public StudentService(AppDbContext context)
    {
        throw new NotImplementedException();
    }

    public async Task<Student> AddAsync(string name, int grade)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Student>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateGradeAsync(int id, int newGrade)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}
