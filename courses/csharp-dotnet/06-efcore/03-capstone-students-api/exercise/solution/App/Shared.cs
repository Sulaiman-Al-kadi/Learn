// Shared.cs is fixed infrastructure — the entity, DbContext, interface, request DTO, and
// controller are already written (all straight from Module 5 + Module 6 lesson 01's patterns).
// You write EfStudentRepository.cs.
using Microsoft.AspNetCore.Mvc;
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

public interface IStudentRepository
{
    Task<List<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(int id);
    Task<Student> AddAsync(string name, int grade);
}

public record CreateStudentRequest(string Name, int Grade);

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentRepository _repository;

    public StudentsController(IStudentRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _repository.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var student = await _repository.GetByIdAsync(id);
        return student is not null ? Ok(student) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name)) return BadRequest("Name is required");
        var student = await _repository.AddAsync(request.Name, request.Grade);
        return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
    }
}
