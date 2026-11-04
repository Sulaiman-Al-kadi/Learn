using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class CorsTests
{
    private static async Task<HttpResponseMessage> From(WebApplicationFactory<Program> f, string origin)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/data");
        request.Headers.Add("Origin", origin);
        return await f.CreateClient().SendAsync(request);
    }

    [Fact] public async Task AllowedOrigin_GetsHeader()
    {
        using var f = new WebApplicationFactory<Program>();
        var response = await From(f, "https://app.example.com");
        Assert.Equal("https://app.example.com", response.Headers.GetValues("Access-Control-Allow-Origin").Single());
    }
    [Fact] public async Task OtherOrigin_NoHeader()
    {
        using var f = new WebApplicationFactory<Program>();
        var response = await From(f, "https://evil.example.com");
        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
    }
}
