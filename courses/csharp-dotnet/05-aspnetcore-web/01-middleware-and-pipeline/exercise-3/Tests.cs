using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class ApiKeyTests
{
    private static HttpClient Client(WebApplicationFactory<Program> f, string? key = null)
    {
        var c = f.CreateClient();
        if (key is not null) c.DefaultRequestHeaders.Add("X-Api-Key", key);
        return c;
    }

    [Fact] public async Task Public_NoKey()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(HttpStatusCode.OK, (await Client(f).GetAsync("/public")).StatusCode);
    }
    [Fact] public async Task Admin_NoKey_401()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(HttpStatusCode.Unauthorized, (await Client(f).GetAsync("/admin/stats")).StatusCode);
    }
    [Fact] public async Task Admin_WrongKey_401()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(HttpStatusCode.Unauthorized, (await Client(f, "nope").GetAsync("/admin/stats")).StatusCode);
    }
    [Fact] public async Task Admin_RightKey_200()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal("stats", await Client(f, "secret").GetStringAsync("/admin/stats"));
    }
}
