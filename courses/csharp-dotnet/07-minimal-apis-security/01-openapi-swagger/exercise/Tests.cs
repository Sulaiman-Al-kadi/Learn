using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class ProgramTests
{
    [Fact]
    public async Task OpenApiDocument_IsReachableAndListsBothRoutes()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");
        response.EnsureSuccessStatusCode();

        string body = await response.Content.ReadAsStringAsync();
        Assert.Contains("/books", body);
        Assert.Contains("GetAllBooks", body);
        Assert.Contains("GetBookById", body);
    }

    [Fact]
    public async Task GetAllBooks_ReturnsExpectedList()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var books = await client.GetFromJsonAsync<List<string>>("/books");

        Assert.Equal(new List<string> { "Dune", "1984", "Foundation" }, books);
    }

    [Fact]
    public async Task GetBookById_ValidIndex_ReturnsTitle()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/books/1");
        response.EnsureSuccessStatusCode();
        string body = await response.Content.ReadAsStringAsync();

        Assert.Contains("1984", body);
    }

    [Fact]
    public async Task GetBookById_OutOfRange_Returns404()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/books/99");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
