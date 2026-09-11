# Module 4 Capstone — HttpClient & a Weather API Client

Everything in this module comes together here: `async`/`await` (lesson 01) to call a real web API without freezing, and JSON deserialization (lesson 02) to turn the response into a C# object. This is precisely the shape of code you'll write to call any external API, and it's the direct ancestor of Module 5's ASP.NET Core (which serves these requests instead of making them).

## `HttpClient` — making a web request

```csharp
using var client = new HttpClient();
client.BaseAddress = new Uri("https://api.example.com");

HttpResponseMessage response = await client.GetAsync("/weather?city=Riyadh");

if (response.IsSuccessStatusCode)
{
    string body = await response.Content.ReadAsStringAsync();
    Console.WriteLine(body);
}
```

| Piece | Meaning |
|---|---|
| `new HttpClient()` | The object that sends HTTP requests. |
| `BaseAddress` | A prefix automatically applied to relative URLs you pass to `GetAsync` etc. |
| `await client.GetAsync(...)` | Sends a `GET` request, **asynchronously** (lesson 01) — the thread isn't blocked waiting for the network. |
| `response.IsSuccessStatusCode` | `true` for 2xx status codes (200 OK, 201 Created, ...); `false` for 4xx/5xx (errors). |
| `response.Content.ReadAsStringAsync()` | Reads the response body as text — also `await`ed, since reading can itself take time. |

## Reading JSON directly — `ReadFromJsonAsync<T>`

Instead of reading the body as a string and then separately calling `JsonSerializer.Deserialize` (lesson 02), `System.Net.Http.Json` provides a shortcut that does both in one step:

```csharp
using System.Net.Http.Json;

public record WeatherResult(string City, double TempC, string Condition);

HttpResponseMessage response = await client.GetAsync("/weather?city=Riyadh");
WeatherResult? weather = await response.Content.ReadFromJsonAsync<WeatherResult>();
```

Same generic-method idea as `Deserialize<T>` — `T` says what shape to build, the result is nullable because the response might not actually match.

## An important design choice: inject the `HttpClient`

```csharp
public class WeatherClient
{
    private readonly HttpClient _http;

    public WeatherClient(HttpClient http)     // constructor injection — Module 2, lesson 08 (DIP)!
    {
        _http = http;
    }

    public async Task<WeatherResult?> GetWeatherAsync(string city)
    {
        HttpResponseMessage response = await _http.GetAsync($"/weather?city={city}");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<WeatherResult>();
    }
}
```

`WeatherClient` doesn't create its own `HttpClient` internally — one is **passed in**. This is exactly Module 2's Dependency Inversion Principle, and it's not just theoretical here: it's what makes this class **testable without a real network call** (see below), and it's how ASP.NET Core's `HttpClientFactory` (Module 5+) will hand you pre-configured clients in real projects.

## Testing HTTP code without the internet — a fake handler

A real API call in a test is slow, flaky (network down? API changed? rate limited?), and non-deterministic — bad qualities for a test. The trick: `HttpClient` doesn't talk to the network directly — it delegates to an `HttpMessageHandler`. Swap in a **fake** one that returns a canned response, and `WeatherClient` behaves identically without any real request ever leaving the machine:

```csharp
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
            Content = new StringContent(_json, Encoding.UTF8, "application/json")
        };
        return Task.FromResult(response);
    }
}
```
```csharp
var handler = new FakeHandler(HttpStatusCode.OK, """{"City":"Riyadh","TempC":38.5,"Condition":"Sunny"}""");
var http = new HttpClient(handler) { BaseAddress = new Uri("https://fake.test") };
var client = new WeatherClient(http);

WeatherResult? result = await client.GetWeatherAsync("Riyadh");   // no real network call happened
```

This is a **realistic professional technique** — you don't need to fully understand `HttpMessageHandler`'s internals right now (the exercise's test file provides this fake for you), just recognize the pattern: *dependency injection makes code testable by letting you substitute a fake for the real thing.*

## Error handling for network calls

```csharp
try
{
    var weather = await client.GetWeatherAsync("Riyadh");
    if (weather is null)
    {
        Console.WriteLine("City not found or request failed.");
    }
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Network error: {ex.Message}");
}
```
`HttpRequestException` covers connection failures (no internet, DNS failure, timeout) — genuinely exceptional situations (Module 2, lesson 07), distinct from a normal "not found" response, which is better modeled by returning `null` (as `GetWeatherAsync` above does) than by throwing.

## Summary
- `HttpClient.GetAsync(url)` sends a request asynchronously; `response.IsSuccessStatusCode` checks for 2xx.
- `response.Content.ReadFromJsonAsync<T>()` combines reading the body and deserializing it, in one awaited call.
- Inject `HttpClient` through a constructor rather than creating one inside your class — this is DIP (Module 2) applied to networking, and it's what makes the class testable.
- Tests can substitute a fake `HttpMessageHandler` to return canned responses instead of making real network calls — fast, reliable, no internet required.
- Model "not found" / "request failed" as a `null` return; reserve exceptions (`HttpRequestException`) for genuine connectivity failures.

## The capstone

Open the exercise. You'll write `WeatherClient` exactly as sketched above. The test file includes the fake handler already built — you just need `WeatherClient` itself to work correctly against it.
