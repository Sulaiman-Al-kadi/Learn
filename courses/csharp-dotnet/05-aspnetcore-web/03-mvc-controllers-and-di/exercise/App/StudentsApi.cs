// Capstone — StudentsApi
// See README.md. Program.cs is already wired up — write everything here.
using Microsoft.AspNetCore.Mvc;

public record Student(int Id, string Name, int Grade);

public interface IStudentRepository
{
    List<Student> GetAll();
    Student? GetById(int id);
    Student Add(string name, int grade);
}

public class InMemoryStudentRepository : IStudentRepository
{
    // TODO: seed with (1, "Ali", 85) and (2, "Sara", 92); track next id starting at 3

    public List<Student> GetAll()
    {
        throw new NotImplementedException();
    }

    public Student? GetById(int id)
    {
        throw new NotImplementedException();
    }

    public Student Add(string name, int grade)
    {
        throw new NotImplementedException();
    }
}

public record CreateStudentRequest(string Name, int Grade);

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    // TODO: private readonly IStudentRepository field, set via constructor

    public StudentsController(IStudentRepository repository)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateStudentRequest request)
    {
        throw new NotImplementedException();
    }
}
