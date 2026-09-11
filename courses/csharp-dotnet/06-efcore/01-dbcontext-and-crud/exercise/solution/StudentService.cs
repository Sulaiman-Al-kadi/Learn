// Exercise 01 — StudentService with EF Core (solution)
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
    private readonly AppDbContext _context;

    public StudentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Student> AddAsync(string name, int grade)
    {
        var student = new Student { Name = name, Grade = grade };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<List<Student>> GetAllAsync()
    {
        return await _context.Students.ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _context.Students.FindAsync(id);
    }

    public async Task<bool> UpdateGradeAsync(int id, int newGrade)
    {
        var student = await _context.Students.FindAsync(id);
        if (student is null) return false;
        student.Grade = newGrade;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student is null) return false;
        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return true;
    }
}
