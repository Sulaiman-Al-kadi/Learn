## Hint 1
`InMemoryStudentRepository`:
```csharp
private readonly List<Student> _students = new List<Student>
{
    new Student(1, "Ali", 85),
    new Student(2, "Sara", 92),
};
private int _nextId = 3;

public List<Student> GetAll() => _students;
public Student? GetById(int id) => _students.FirstOrDefault(s => s.Id == id);
public Student Add(string name, int grade)
{
    var student = new Student(_nextId++, name, grade);
    _students.Add(student);
    return student;
}
```

## Hint 2
`StudentsController`'s constructor: `private readonly IStudentRepository _repository; public StudentsController(IStudentRepository repository) { _repository = repository; }`

## Hint 3
```csharp
[HttpGet]
public IActionResult GetAll() => Ok(_repository.GetAll());

[HttpGet("{id:int}")]
public IActionResult GetById(int id)
{
    var student = _repository.GetById(id);
    return student is not null ? Ok(student) : NotFound();
}
```

## Hint 4
```csharp
[HttpPost]
public IActionResult Create([FromBody] CreateStudentRequest request)
{
    if (string.IsNullOrWhiteSpace(request.Name)) return BadRequest("Name is required");
    var student = _repository.Add(request.Name, request.Grade);
    return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
}
```
