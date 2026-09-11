using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class TaskApiAuthTests
{
    public record LoginRequest(string Username, string Password);

    private static async Task<HttpClient> LoggedInClientAsync(WebApplicationFactory<Program> factory, string username, string password)
    {
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/login", new LoginRequest(username, password));
        string token = (await response.Content.ReadAsStringAsync()).Trim('"');
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    [Fact]
    public async Task Tasks_AsUser_Returns200()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = await LoggedInClientAsync(factory, "ali", "secret123");

        var response = await client.GetAsync("/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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
    public async Task DeleteTask_AsUser_Returns403()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = await LoggedInClientAsync(factory, "ali", "secret123");

        var response = await client.DeleteAsync("/tasks/1");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTask_AsAdmin_Returns204()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = await LoggedInClientAsync(factory, "admin", "adminpass");

        var response = await client.DeleteAsync("/tasks/1");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTask_WithoutToken_Returns401()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.DeleteAsync("/tasks/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Cors_PreflightFromAllowedOrigin_ReturnsAllowOriginHeader()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Options, "/tasks");
        request.Headers.Add("Origin", "https://example.com");
        request.Headers.Add("Access-Control-Request-Method", "GET");

        var response = await client.SendAsync(request);

        Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"));
    }
}
