using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class WhoAmITests
{
    private static async Task<HttpClient> LoginAs(WebApplicationFactory<Program> f, string user)
    {
        var c = f.CreateClient();
        var response = await c.PostAsJsonAsync("/login", new { username = user });
        string token = (await response.Content.ReadAsStringAsync()).Trim('"');
        c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return c;
    }

    [Fact] public async Task Me_NoToken_401()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(HttpStatusCode.Unauthorized, (await f.CreateClient().GetAsync("/me")).StatusCode);
    }
    [Fact] public async Task Me_BadToken_401()
    {
        using var f = new WebApplicationFactory<Program>();
        var c = f.CreateClient();
        c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "not.a.token");
        Assert.Equal(HttpStatusCode.Unauthorized, (await c.GetAsync("/me")).StatusCode);
    }
    [Fact] public async Task Me_ReturnsName()
    {
        using var f = new WebApplicationFactory<Program>();
        var c = await LoginAs(f, "sara");
        Assert.Contains("sara", await c.GetStringAsync("/me"));
    }
}
