using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class NotesTests
{
    public record Note(int Id, string Text);

    [Fact] public async Task Post_201()
    {
        using var f = new WebApplicationFactory<Program>();
        var response = await f.CreateClient().PostAsJsonAsync("/notes", new { text = "hi" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    [Fact] public async Task List_NewestFirst()
    {
        using var f = new WebApplicationFactory<Program>();
        var c = f.CreateClient();
        await c.PostAsJsonAsync("/notes", new { text = "one" });
        await c.PostAsJsonAsync("/notes", new { text = "two" });
        Assert.Equal("two", (await c.GetFromJsonAsync<List<Note>>("/notes"))![0].Text);
    }
    [Fact] public async Task GetOne_Missing_404()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(HttpStatusCode.NotFound, (await f.CreateClient().GetAsync("/notes/9")).StatusCode);
    }
}
