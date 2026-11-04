using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Text.Json;

public class OpenApiTests
{
    private static async Task<JsonElement> GetAsync()
    {
        using var f = new WebApplicationFactory<Program>();
        var doc = JsonDocument.Parse(await f.CreateClient().GetStringAsync("/openapi/v1.json"));
        return doc.RootElement.GetProperty("paths").GetProperty("/books").GetProperty("get").Clone();
    }

    [Fact] public async Task HasOperationId() => Assert.Equal("ListBooks", (await GetAsync()).GetProperty("operationId").GetString());
    [Fact] public async Task HasSummary() => Assert.Equal("List every book", (await GetAsync()).GetProperty("summary").GetString());
    [Fact] public async Task HasTag() => Assert.Equal("Books", (await GetAsync()).GetProperty("tags")[0].GetString());
}
