using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class LifetimeTests
{
    public record Ids(Guid Session, Guid Request);

    [Fact] public async Task Singleton_StaysTheSame()
    {
        using var f = new WebApplicationFactory<Program>();
        var c = f.CreateClient();
        var a = await c.GetFromJsonAsync<Ids>("/ids");
        var b = await c.GetFromJsonAsync<Ids>("/ids");
        Assert.Equal(a!.Session, b!.Session);
    }
    [Fact] public async Task Scoped_ChangesPerRequest()
    {
        using var f = new WebApplicationFactory<Program>();
        var c = f.CreateClient();
        var a = await c.GetFromJsonAsync<Ids>("/ids");
        var b = await c.GetFromJsonAsync<Ids>("/ids");
        Assert.NotEqual(a!.Request, b!.Request);
    }
}
