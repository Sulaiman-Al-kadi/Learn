using System.Net;
using System.Text;
using Xunit;

public class QuoteClientTests
{
    private sealed class Fake(HttpStatusCode status, string body) : HttpMessageHandler
    {
        public string? Path;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            Path = request.RequestUri!.AbsolutePath;
            return Task.FromResult(new HttpResponseMessage(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") });
        }
    }

    private static QuoteClient Client(Fake fake) => new(new HttpClient(fake) { BaseAddress = new Uri("https://api.test") });

    [Fact] public async Task Success_ReturnsQuote()
    {
        var fake = new Fake(HttpStatusCode.OK, "{\"id\":7,\"text\":\"Keep going\"}");
        Assert.Equal(new Quote(7, "Keep going"), await Client(fake).GetAsync(7));
        Assert.Equal("/quotes/7", fake.Path);
    }
    [Fact] public async Task NotFound_ReturnsNull() => Assert.Null(await Client(new Fake(HttpStatusCode.NotFound, "")).GetAsync(1));
    [Fact] public async Task ServerError_Throws() => await Assert.ThrowsAsync<HttpRequestException>(() => Client(new Fake(HttpStatusCode.InternalServerError, "")).GetAsync(1));
}
