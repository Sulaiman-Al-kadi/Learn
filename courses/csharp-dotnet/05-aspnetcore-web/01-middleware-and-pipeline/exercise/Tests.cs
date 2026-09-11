using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public ProgramTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task Hello_ReturnsGreetingText()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/hello");

        response.EnsureSuccessStatusCode();
        string body = await response.Content.ReadAsStringAsync();
        Assert.Equal("Hello, World!", body);
    }

    [Fact]
    public async Task Hello_ResponseHasCustomHeader()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/hello");

        Assert.True(response.Headers.TryGetValues("X-Powered-By", out var values));
        Assert.Equal("LearnLab", values!.First());
    }

    [Fact]
    public async Task Students_ReturnsExpectedList()
    {
        var client = _factory.CreateClient();
        var students = await client.GetFromJsonAsync<List<string>>("/students");

        Assert.NotNull(students);
        Assert.Equal(new List<string> { "Ali", "Sara", "Omar" }, students);
    }

    [Fact]
    public async Task Echo_ReturnsRouteParameter()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/echo/hello123");

        response.EnsureSuccessStatusCode();
        string body = await response.Content.ReadAsStringAsync();
        Assert.Equal("hello123", body);
    }

    [Fact]
    public async Task UnmappedRoute_Returns404()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
