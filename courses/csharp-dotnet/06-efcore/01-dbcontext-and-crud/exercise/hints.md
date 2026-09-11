## Hint 1
Constructor: `private readonly AppDbContext _context; public StudentService(AppDbContext context) { _context = context; }`

## Hint 2
```csharp
public async Task<Student> AddAsync(string name, int grade)
{
    var student = new Student { Name = name, Grade = grade };
    _context.Students.Add(student);
    await _context.SaveChangesAsync();
    return student;
}
```

## Hint 3
`GetAllAsync`: `return await _context.Students.ToListAsync();`
`GetByIdAsync`: `return await _context.Students.FindAsync(id);`

## Hint 4
`UpdateGradeAsync` and `DeleteAsync` share a shape — find first, check for null, act, save:
```csharp
public async Task<bool> UpdateGradeAsync(int id, int newGrade)
{
    var student = await _context.Students.FindAsync(id);
    if (student is null) return false;
    student.Grade = newGrade;
    await _context.SaveChangesAsync();
    return true;
}
```
