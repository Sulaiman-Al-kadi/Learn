## Hint 1
This is nearly identical to lesson 01's `StudentService`: `private readonly AppDbContext _context; public EfStudentRepository(AppDbContext context) { _context = context; }`

## Hint 2
```csharp
public async Task<List<Student>> GetAllAsync() => await _context.Students.ToListAsync();
public async Task<Student?> GetByIdAsync(int id) => await _context.Students.FindAsync(id);
```

## Hint 3
```csharp
public async Task<Student> AddAsync(string name, int grade)
{
    var student = new Student { Name = name, Grade = grade };
    _context.Students.Add(student);
    await _context.SaveChangesAsync();
    return student;
}
```
