using System.Net;
using System.Text;
using Xunit;

public class TodoClientTests
{
    private sealed class Fake(HttpStatusCode status) : HttpMessageHandler
    {
        public HttpMethod? Method; public string? Path; public string? Body;
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            Method = request.Method; Path = request.RequestUri!.AbsolutePath;
            Body = request.Content is null ? null : await request.Content.ReadAsStringAsync(token);
            return new HttpResponseMessage(status) { Content = new StringContent("{\"id\":5,\"title\":\"Read\",\"done\":false}", Encoding.UTF8, "application/json") };
        }
    }

    private static TodoClient Client(Fake fake) => new(new HttpClient(fake) { BaseAddress = new Uri("https://api.test") });

    [Fact] public async Task Create_PostsJson()
    {
        var fake = new Fake(HttpStatusCode.Created);
        await Client(fake).CreateAsync("Read");
        Assert.Equal(HttpMethod.Post, fake.Method);
        Assert.Equal("/todos", fake.Path);
        Assert.Contains("\"Read\"", fake.Body);
    }
    [Fact] public async Task Create_ReturnsServerId() => Assert.Equal(5, (await Client(new Fake(HttpStatusCode.Created)).CreateAsync("Read")).Id);
    [Fact] public async Task Create_ThrowsOnFailure() => await Assert.ThrowsAsync<HttpRequestException>(() => Client(new Fake(HttpStatusCode.BadRequest)).CreateAsync("x"));
}
