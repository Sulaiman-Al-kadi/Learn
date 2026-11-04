using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class NotesEditTests
{
    public record Note(int Id, string Text);

    [Fact] public async Task Put_Updates()
    {
        using var f = new WebApplicationFactory<Program>();
        var c = f.CreateClient();
        Assert.Equal(HttpStatusCode.NoContent, (await c.PutAsJsonAsync("/notes/1", new { text = "new" })).StatusCode);
        Assert.Equal("new", (await c.GetFromJsonAsync<List<Note>>("/notes"))![0].Text);
    }
    [Fact] public async Task Put_Missing_404()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(HttpStatusCode.NotFound, (await f.CreateClient().PutAsJsonAsync("/notes/9", new { text = "x" })).StatusCode);
    }
    [Fact] public async Task Delete_Removes()
    {
        using var f = new WebApplicationFactory<Program>();
        var c = f.CreateClient();
        Assert.Equal(HttpStatusCode.NoContent, (await c.DeleteAsync("/notes/1")).StatusCode);
        Assert.Equal(2, (await c.GetFromJsonAsync<List<Note>>("/notes"))!.Count);
    }
    [Fact] public async Task Delete_Missing_404()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(HttpStatusCode.NotFound, (await f.CreateClient().DeleteAsync("/notes/9")).StatusCode);
    }
    [Fact] public async Task Paging()
    {
        using var f = new WebApplicationFactory<Program>();
        var page = await f.CreateClient().GetFromJsonAsync<List<Note>>("/notes?page=2&size=2");
        Assert.Equal("c", page!.Single().Text);
    }
}
