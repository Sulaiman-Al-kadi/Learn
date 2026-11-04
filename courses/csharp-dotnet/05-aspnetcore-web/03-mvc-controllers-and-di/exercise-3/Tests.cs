using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class ProductsTests
{
    public record Product(int Id, string Name);

    [Fact] public async Task GetAll()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Single((await f.CreateClient().GetFromJsonAsync<List<Product>>("/products"))!);
    }
    [Fact] public async Task GetOne_Missing_404()
    {
        using var f = new WebApplicationFactory<Program>();
        Assert.Equal(HttpStatusCode.NotFound, (await f.CreateClient().GetAsync("/products/9")).StatusCode);
    }
    [Fact] public async Task Post_201_ThenReadable()
    {
        using var f = new WebApplicationFactory<Program>();
        var c = f.CreateClient();
        var response = await c.PostAsJsonAsync("/products", new { name = "Ink" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Ink", (await c.GetFromJsonAsync<Product>("/products/2"))!.Name);
    }
}
