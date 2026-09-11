using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

// Swaps the real "students.db" SQLite file for a private in-memory SQLite connection during
// tests — see the lesson for why. One instance per test, so every test gets a clean database.
public class TestingFactory : WebApplicationFactory<Program>, IDisposable
{
    private readonly SqliteConnection _connection = new SqliteConnection("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _connection.Dispose();
    }
}

public class StudentsApiTests
{
    public record Student(int Id, string Name, int Grade);
    public record CreateStudentRequest(string Name, int Grade);

    [Fact]
    public async Task GetAll_EmptyDatabase_ReturnsEmptyList()
    {
        using var factory = new TestingFactory();
        var client = factory.CreateClient();

        var students = await client.GetFromJsonAsync<List<Student>>("/api/students");

        Assert.NotNull(students);
        Assert.Empty(students!);
    }

    [Fact]
    public async Task Post_ThenGetAll_ReturnsCreatedStudent()
    {
        using var factory = new TestingFactory();
        var client = factory.CreateClient();

        await client.PostAsJsonAsync("/api/students", new CreateStudentRequest("Ali", 85));
        var students = await client.GetFromJsonAsync<List<Student>>("/api/students");

        Assert.Single(students!);
        Assert.Equal("Ali", students![0].Name);
    }

    [Fact]
    public async Task Post_ReturnsCreated201WithLocation()
    {
        using var factory = new TestingFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/students", new CreateStudentRequest("Sara", 92));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task GetById_Found_ReturnsStudent()
    {
        using var factory = new TestingFactory();
        var client = factory.CreateClient();

        var created = await (await client.PostAsJsonAsync("/api/students", new CreateStudentRequest("Omar", 70)))
            .Content.ReadFromJsonAsync<Student>();

        var fetched = await client.GetFromJsonAsync<Student>($"/api/students/{created!.Id}");

        Assert.Equal("Omar", fetched!.Name);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        using var factory = new TestingFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/students/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_BlankName_ReturnsBadRequest()
    {
        using var factory = new TestingFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/students", new CreateStudentRequest("", 50));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task EachTest_GetsAFreshDatabase()
    {
        // A separate test creating a student — if databases leaked between tests, this
        // fresh factory would incorrectly see students from OTHER tests too.
        using var factory = new TestingFactory();
        var client = factory.CreateClient();

        var students = await client.GetFromJsonAsync<List<Student>>("/api/students");

        Assert.Empty(students!);
    }
}
