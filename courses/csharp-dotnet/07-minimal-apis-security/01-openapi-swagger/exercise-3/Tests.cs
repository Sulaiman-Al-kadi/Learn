using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Text.Json;

public class ResponsesTests
{
    [Fact] public async Task Doc_ListsBothResponses()
    {
        using var f = new WebApplicationFactory<Program>();
        var doc = JsonDocument.Parse(await f.CreateClient().GetStringAsync("/openapi/v1.json"));
        var responses = doc.RootElement.GetProperty("paths").GetProperty("/books/{id}").GetProperty("get").GetProperty("responses");
        Assert.True(responses.TryGetProperty("200", out _));
        Assert.True(responses.TryGetProperty("404", out _));
    }
    [Fact] public async Task Found_200()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(HttpStatusCode.OK, (await f.CreateClient().GetAsync("/books/1")).StatusCode);
    }
    [Fact] public async Task Missing_404()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(HttpStatusCode.NotFound, (await f.CreateClient().GetAsync("/books/2")).StatusCode);
    }
}
