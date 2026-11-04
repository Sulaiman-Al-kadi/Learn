using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class RequestCounterTests
{
    [Fact] public async Task Counts_EachRequest()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();
        await client.GetAsync("/ping");
        var second = await client.GetAsync("/ping");
        Assert.Equal("2", second.Headers.GetValues("X-Request-Count").Single());
    }
    [Fact] public async Task Counts_UnknownRoutesToo()
    {
        using var factory = new WebApplicationFactory<Program>();
        var response = await factory.CreateClient().GetAsync("/nope");
        Assert.Equal("1", response.Headers.GetValues("X-Request-Count").Single());
    }
    [Fact] public async Task Pipeline_StillRuns()
    {
        using var factory = new WebApplicationFactory<Program>();
        Assert.Equal("pong", await factory.CreateClient().GetStringAsync("/ping"));
    }
}
