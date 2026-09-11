using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class TaskApiTests
{
    public record LoginRequest(string Username, string Password);

    private static async Task<HttpClient> LoggedInClientAsync(WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/login", new LoginRequest("ali", "secret123"));
        string token = (await response.Content.ReadAsStringAsync()).Trim('"');
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/login", new LoginRequest("ali", "secret123"));

        response.EnsureSuccessStatusCode();
        string token = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/login", new LoginRequest("ali", "wrongpassword"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Tasks_WithoutToken_Returns401()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/tasks");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Tasks_WithValidToken_Returns200AndTaskList()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = await LoggedInClientAsync(factory);

        var tasks = await client.GetFromJsonAsync<List<string>>("/tasks");

        Assert.Equal(new List<string> { "Buy milk", "Write report" }, tasks);
    }

    [Fact]
    public async Task WhoAmI_WithValidToken_ReturnsUsername()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = await LoggedInClientAsync(factory);

        var response = await client.GetAsync("/tasks/whoami");
        response.EnsureSuccessStatusCode();
        string body = await response.Content.ReadAsStringAsync();

        Assert.Contains("ali", body);
    }

    [Fact]
    public async Task WhoAmI_WithoutToken_Returns401()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/tasks/whoami");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
