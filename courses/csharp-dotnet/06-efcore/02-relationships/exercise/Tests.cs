using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class ClassroomServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly ClassroomService _service;

    public ClassroomServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
        _service = new ClassroomService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
    }

    [Fact]
    public async Task CreateClassroomAsync_PersistsAndAssignsId()
    {
        var classroom = await _service.CreateClassroomAsync("Grade 10A");
        Assert.True(classroom.Id > 0);
    }

    [Fact]
    public async Task AddStudentAsync_LinksToClassroom()
    {
        var classroom = await _service.CreateClassroomAsync("Grade 10A");
        var student = await _service.AddStudentAsync(classroom.Id, "Ali");

        Assert.Equal(classroom.Id, student.ClassroomId);
    }

    [Fact]
    public async Task GetClassroomWithStudentsAsync_EagerLoadsStudents()
    {
        var classroom = await _service.CreateClassroomAsync("Grade 10A");
        await _service.AddStudentAsync(classroom.Id, "Ali");
        await _service.AddStudentAsync(classroom.Id, "Sara");

        var loaded = await _service.GetClassroomWithStudentsAsync(classroom.Id);

        Assert.NotNull(loaded);
        Assert.Equal(2, loaded!.Students.Count);
    }

    [Fact]
    public async Task WithoutInclude_StudentsNavigationIsEmpty_IllustratesTheTrap()
    {
        // This test queries with a FRESH DbContext (same underlying database, no tracked
        // entities to auto-fix-up from) to demonstrate the lesson's point: no Include means
        // an empty navigation property, even though matching rows genuinely exist.
        var classroom = await _service.CreateClassroomAsync("Grade 10A");
        await _service.AddStudentAsync(classroom.Id, "Ali");

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
        using var freshContext = new AppDbContext(options);
        var withoutInclude = await freshContext.Classrooms.FirstAsync(c => c.Id == classroom.Id);

        Assert.Empty(withoutInclude.Students);
    }

    [Fact]
    public async Task GetStudentsInClassroomAsync_ReturnsOnlyThatClassroomsStudents()
    {
        var classroomA = await _service.CreateClassroomAsync("Grade 10A");
        var classroomB = await _service.CreateClassroomAsync("Grade 10B");
        await _service.AddStudentAsync(classroomA.Id, "Ali");
        await _service.AddStudentAsync(classroomA.Id, "Sara");
        await _service.AddStudentAsync(classroomB.Id, "Omar");

        var studentsInA = await _service.GetStudentsInClassroomAsync(classroomA.Id);

        Assert.Equal(2, studentsInA.Count);
        Assert.DoesNotContain(studentsInA, s => s.Name == "Omar");
    }

    [Fact]
    public async Task DeleteClassroomAsync_Found_CascadesAndReturnsTrue()
    {
        var classroom = await _service.CreateClassroomAsync("Grade 10A");
        await _service.AddStudentAsync(classroom.Id, "Ali");

        bool result = await _service.DeleteClassroomAsync(classroom.Id);

        Assert.True(result);
        var remainingStudents = await _context.Students.Where(s => s.ClassroomId == classroom.Id).ToListAsync();
        Assert.Empty(remainingStudents);
    }

    [Fact]
    public async Task DeleteClassroomAsync_NotFound_ReturnsFalse()
    {
        bool result = await _service.DeleteClassroomAsync(999);
        Assert.False(result);
    }
}
