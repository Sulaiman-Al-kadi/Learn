using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class RoleTests
{
    private static async Task<HttpClient> LoginAs(WebApplicationFactory<Program> f, string user)
    {
        var c = f.CreateClient();
        var response = await c.PostAsJsonAsync("/login", new { username = user });
        string token = (await response.Content.ReadAsStringAsync()).Trim('"');
        c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return c;
    }

    [Fact] public async Task Get_Anonymous_401()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(HttpStatusCode.Unauthorized, (await f.CreateClient().GetAsync("/reports")).StatusCode);
    }
    [Fact] public async Task Get_User_200()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(HttpStatusCode.OK, (await (await LoginAs(f, "ali")).GetAsync("/reports")).StatusCode);
    }
    [Fact] public async Task Delete_User_403()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(HttpStatusCode.Forbidden, (await (await LoginAs(f, "ali")).DeleteAsync("/reports")).StatusCode);
    }
    [Fact] public async Task Delete_Admin_200()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(HttpStatusCode.OK, (await (await LoginAs(f, "sara")).DeleteAsync("/reports")).StatusCode);
    }
}
