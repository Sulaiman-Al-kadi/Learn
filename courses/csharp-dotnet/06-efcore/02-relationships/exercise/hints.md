## Hint 1
`CreateClassroomAsync`/`AddStudentAsync` follow the exact same shape as lesson 01's `AddAsync`: construct, `Add`, `SaveChangesAsync`, return.

## Hint 2
`GetClassroomWithStudentsAsync` — the `Include` is the whole point:
```csharp
return await _context.Classrooms
    .Include(c => c.Students)
    .FirstOrDefaultAsync(c => c.Id == classroomId);
```

## Hint 3
`GetStudentsInClassroomAsync`: `return await _context.Students.Where(s => s.ClassroomId == classroomId).ToListAsync();`

## Hint 4
`DeleteClassroomAsync` is the same find-check-act-save pattern as lesson 01's delete/update methods:
```csharp
var classroom = await _context.Classrooms.FindAsync(classroomId);
if (classroom is null) return false;
_context.Classrooms.Remove(classroom);
await _context.SaveChangesAsync();
return true;
```
