using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

// Fresh factory per test — the singleton repository's state must not leak between tests.
public class StudentsApiTests
{
    public record Student(int Id, string Name, int Grade);
    public record CreateStudentRequest(string Name, int Grade);

    [Fact]
    public async Task GetAll_ReturnsSeededStudents()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var students = await client.GetFromJsonAsync<List<Student>>("/api/students");

        Assert.NotNull(students);
        Assert.Equal(2, students!.Count);
        Assert.Contains(students, s => s.Name == "Ali" && s.Grade == 85);
        Assert.Contains(students, s => s.Name == "Sara" && s.Grade == 92);
    }

    [Fact]
    public async Task GetById_Found_ReturnsStudent()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var student = await client.GetFromJsonAsync<Student>("/api/students/1");

        Assert.NotNull(student);
        Assert.Equal("Ali", student!.Name);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/students/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ValidStudent_Creates201WithLocationHeader()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/students", new CreateStudentRequest("Omar", 70));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<Student>();
        Assert.NotNull(created);
        Assert.Equal("Omar", created!.Name);
        Assert.Equal(3, created.Id);
    }

    [Fact]
    public async Task Post_ThenGetById_SeesTheNewStudent()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        await client.PostAsJsonAsync("/api/students", new CreateStudentRequest("Lina", 88));
        var student = await client.GetFromJsonAsync<Student>("/api/students/3");

        Assert.NotNull(student);
        Assert.Equal("Lina", student!.Name);
    }

    [Fact]
    public async Task Post_BlankName_ReturnsBadRequest()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/students", new CreateStudentRequest("", 50));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
