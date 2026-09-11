using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class StudentsApiTests
{
    public record Student(int Id, string Name, int Grade);

    // ---------- Integration tests: check actual values ----------

    [Fact]
    public async Task GetAll_ReturnsSeededStudents()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var students = await client.GetFromJsonAsync<List<Student>>("/api/students");

        Assert.Equal(2, students!.Count);
        Assert.Contains(students, s => s.Name == "Ali" && s.Grade == 85);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/students/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ---------- Contract tests: check response SHAPE, not specific values ----------

    [Fact]
    public async Task GetById_ResponseHasRequiredContractFields()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/students/1");
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = doc.RootElement;

        Assert.True(root.TryGetProperty("id", out var id) && id.ValueKind == JsonValueKind.Number);
        Assert.True(root.TryGetProperty("name", out var name) && name.ValueKind == JsonValueKind.String);
        Assert.True(root.TryGetProperty("grade", out var grade) && grade.ValueKind == JsonValueKind.Number);
    }

    [Fact]
    public async Task GetAll_EveryElementHasRequiredContractFields()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/students");
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = doc.RootElement;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        Assert.True(root.GetArrayLength() > 0);
        foreach (var element in root.EnumerateArray())
        {
            Assert.True(element.TryGetProperty("id", out _));
            Assert.True(element.TryGetProperty("name", out _));
            Assert.True(element.TryGetProperty("grade", out _));
        }
    }
}
