using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

// Each test creates its OWN WebApplicationFactory, so the in-memory todo list
// starts fresh (back to the 3 seeded items) every time — tests never interfere
// with each other's data, even though POST mutates state.
public class TodoApiTests
{
    public record TodoItem(int Id, string Title, bool Done);
    public record CreateTodoRequest(string Title);

    [Fact]
    public async Task GetAll_ReturnsSeededItems()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var todos = await client.GetFromJsonAsync<List<TodoItem>>("/todos");

        Assert.NotNull(todos);
        Assert.Equal(3, todos!.Count);
        Assert.Contains(todos, t => t.Title == "Buy milk");
    }

    [Fact]
    public async Task GetById_Found_ReturnsItem()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var todo = await client.GetFromJsonAsync<TodoItem>("/todos/2");

        Assert.NotNull(todo);
        Assert.Equal("Write report", todo!.Title);
        Assert.True(todo.Done);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/todos/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_NonNumeric_Returns404DueToRouteConstraint()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/todos/abc");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ValidTitle_Creates201AndAddsItem()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/todos", new CreateTodoRequest("Read a book"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.NotNull(created);
        Assert.Equal("Read a book", created!.Title);
        Assert.False(created.Done);
        Assert.Equal(4, created.Id);

        var all = await client.GetFromJsonAsync<List<TodoItem>>("/todos");
        Assert.Equal(4, all!.Count);
    }

    [Fact]
    public async Task Post_BlankTitle_ReturnsBadRequest()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/todos", new CreateTodoRequest("   "));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Search_CaseInsensitive_ReturnsMatches()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var results = await client.GetFromJsonAsync<List<TodoItem>>("/todos/search?title=MILK");

        Assert.NotNull(results);
        Assert.Single(results!);
        Assert.Equal("Buy milk", results![0].Title);
    }

    [Fact]
    public async Task Search_NoMatches_ReturnsEmptyList()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var results = await client.GetFromJsonAsync<List<TodoItem>>("/todos/search?title=zzz-nope");

        Assert.NotNull(results);
        Assert.Empty(results!);
    }
}
