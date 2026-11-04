using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class CalculatorTests
{
    [Fact] public async Task Add_FromRoute()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(7, await f.CreateClient().GetFromJsonAsync<int>("/add/3/4"));
    }
    [Fact] public async Task Divide_FromQuery()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(5, await f.CreateClient().GetFromJsonAsync<int>("/divide?a=10&b=2"));
    }
    [Fact] public async Task Divide_ByZero_400()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(HttpStatusCode.BadRequest, (await f.CreateClient().GetAsync("/divide?a=1&b=0")).StatusCode);
    }
    [Fact] public async Task Sum_FromBody()
    {
        using var f = new WebApplicationFactory<Program>();
        var response = await f.CreateClient().PostAsJsonAsync("/sum", new[] { 1, 2, 3 });
        Assert.Equal(6, await response.Content.ReadFromJsonAsync<int>());
    }
}
