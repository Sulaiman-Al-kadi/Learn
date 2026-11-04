using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class PasswordTests
{
    [Theory] [InlineData("sara", "wrong")] [InlineData("nobody", "pass123")]
    public async Task BadLogin_401(string user, string password)
    {
        using var f = new WebApplicationFactory<Program>();
        var response = await f.CreateClient().PostAsJsonAsync("/login", new { username = user, password });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    [Fact] public async Task Secret_NoToken_401()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(HttpStatusCode.Unauthorized, (await f.CreateClient().GetAsync("/secret")).StatusCode);
    }
    [Fact] public async Task GoodLogin_OpensSecret()
    {
        using var f = new WebApplicationFactory<Program>();
        var c = f.CreateClient();
        var response = await c.PostAsJsonAsync("/login", new { username = "sara", password = "pass123" });
        string token = (await response.Content.ReadFromJsonAsync<string>())!;
        c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        Assert.Contains("the secret", await c.GetStringAsync("/secret"));
    }
}
