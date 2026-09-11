// Capstone — EfStudentRepository
// See README.md.
using Microsoft.EntityFrameworkCore;

public class EfStudentRepository : IStudentRepository
{
    // TODO: private readonly AppDbContext field, set via constructor
    public EfStudentRepository(AppDbContext context)
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

    public async Task<Student> AddAsync(string name, int grade)
    {
        throw new NotImplementedException();
    }
}
