// Capstone — EfStudentRepository (solution)
using Microsoft.EntityFrameworkCore;

public class EfStudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;

    public EfStudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Student>> GetAllAsync()
    {
        return await _context.Students.ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _context.Students.FindAsync(id);
    }

    public async Task<Student> AddAsync(string name, int grade)
    {
        var student = new Student { Name = name, Grade = grade };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }
}
