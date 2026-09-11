// Exercise 02 — Classroom/Student relationship (solution)
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
    private readonly AppDbContext _context;

    public ClassroomService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Classroom> CreateClassroomAsync(string name)
    {
        var classroom = new Classroom { Name = name };
        _context.Classrooms.Add(classroom);
        await _context.SaveChangesAsync();
        return classroom;
    }

    public async Task<Student> AddStudentAsync(int classroomId, string name)
    {
        var student = new Student { Name = name, ClassroomId = classroomId };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<Classroom?> GetClassroomWithStudentsAsync(int classroomId)
    {
        return await _context.Classrooms
            .Include(c => c.Students)
            .FirstOrDefaultAsync(c => c.Id == classroomId);
    }

    public async Task<List<Student>> GetStudentsInClassroomAsync(int classroomId)
    {
        return await _context.Students.Where(s => s.ClassroomId == classroomId).ToListAsync();
    }

    public async Task<bool> DeleteClassroomAsync(int classroomId)
    {
        var classroom = await _context.Classrooms.FindAsync(classroomId);
        if (classroom is null) return false;
        _context.Classrooms.Remove(classroom);
        await _context.SaveChangesAsync();
        return true;
    }
}
