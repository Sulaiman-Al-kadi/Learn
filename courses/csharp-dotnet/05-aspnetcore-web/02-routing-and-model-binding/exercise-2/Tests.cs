using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class BooksTests
{
    public record Book(int Id, string Title, string Author);

    [Fact] public async Task ById_Found()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal("Emma", (await f.CreateClient().GetFromJsonAsync<Book>("/books/2"))!.Title);
    }
    [Fact] public async Task ById_Missing_404()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(HttpStatusCode.NotFound, (await f.CreateClient().GetAsync("/books/9")).StatusCode);
    }
    [Fact] public async Task ById_NotANumber_404()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(HttpStatusCode.NotFound, (await f.CreateClient().GetAsync("/books/abc")).StatusCode);
    }
    [Fact] public async Task Filter_IgnoresCase()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(2, (await f.CreateClient().GetFromJsonAsync<List<Book>>("/books?author=austen"))!.Count);
    }
    [Fact] public async Task NoFilter_All()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(3, (await f.CreateClient().GetFromJsonAsync<List<Book>>("/books"))!.Count);
    }
}
