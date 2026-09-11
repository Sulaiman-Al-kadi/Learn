using System.Net;
using System.Text;
using Xunit;

// A fake HttpMessageHandler — HttpClient delegates all real network work to a handler like this.
// Substituting this one means WeatherClient never makes a real network call during tests.
public class FakeHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _status;
    private readonly string _json;

    public FakeHandler(HttpStatusCode status, string json)
    {
        _status = status;
        _json = json;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(_status)
        {
            Content = new StringContent(_json, Encoding.UTF8, "application/json"),
        };
        return Task.FromResult(response);
    }
}

public class WeatherClientTests
{
    private static WeatherClient MakeClient(HttpStatusCode status, string json)
    {
        var handler = new FakeHandler(status, json);
        var http = new HttpClient(handler) { BaseAddress = new Uri("https://fake.test") };
        return new WeatherClient(http);
    }

    [Fact]
    public async Task GetWeatherAsync_Success_ReturnsParsedWeather()
    {
        var client = MakeClient(HttpStatusCode.OK, """{"City":"Riyadh","TempC":38.5,"Condition":"Sunny"}""");

        WeatherResult? result = await client.GetWeatherAsync("Riyadh");

        Assert.NotNull(result);
        Assert.Equal("Riyadh", result!.City);
        Assert.Equal(38.5, result.TempC);
        Assert.Equal("Sunny", result.Condition);
    }

    [Fact]
    public async Task GetWeatherAsync_DifferentPayload_ParsesCorrectly()
    {
        var client = MakeClient(HttpStatusCode.OK, """{"City":"London","TempC":12.0,"Condition":"Rainy"}""");

        WeatherResult? result = await client.GetWeatherAsync("London");

        Assert.NotNull(result);
        Assert.Equal("London", result!.City);
        Assert.Equal(12.0, result.TempC);
        Assert.Equal("Rainy", result.Condition);
    }

    [Fact]
    public async Task GetWeatherAsync_NotFound_ReturnsNull()
    {
        var client = MakeClient(HttpStatusCode.NotFound, "");

        WeatherResult? result = await client.GetWeatherAsync("Nowhere");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetWeatherAsync_ServerError_ReturnsNull()
    {
        var client = MakeClient(HttpStatusCode.InternalServerError, "");

        WeatherResult? result = await client.GetWeatherAsync("Riyadh");

        Assert.Null(result);
    }
}
