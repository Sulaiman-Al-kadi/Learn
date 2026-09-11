# Capstone — WeatherClient

Write everything in `WeatherClient.cs`.

## `WeatherResult` (record)
```csharp
public record WeatherResult(string City, double TempC, string Condition);
```

## `WeatherClient`
- Constructor `WeatherClient(HttpClient http)` — store the injected client (don't create your own `HttpClient` inside this class).
- `async Task<WeatherResult?> GetWeatherAsync(string city)`:
  1. `await` a `GET` request to `$"/weather?city={city}"` using the injected client.
  2. If the response is **not** successful (`!response.IsSuccessStatusCode`), return `null`.
  3. Otherwise, `await` reading the response body as a `WeatherResult` using `ReadFromJsonAsync<WeatherResult>()`, and return it.

## How this gets tested — read but don't edit

`Tests.cs` includes a `FakeHandler` (an `HttpMessageHandler`) that returns a canned JSON response instead of making a real network call — see the lesson for why this pattern exists. You don't need to modify or fully understand `FakeHandler`; just know your `WeatherClient` will be constructed with an `HttpClient` wired to it, e.g.:

```csharp
var handler = new FakeHandler(HttpStatusCode.OK, """{"City":"Riyadh","TempC":38.5,"Condition":"Sunny"}""");
var http = new HttpClient(handler) { BaseAddress = new Uri("https://fake.test") };
var client = new WeatherClient(http);
```

## Rules
- `GetWeatherAsync` must be `async` and return `Task<WeatherResult?>`.
- Use `await` for both the HTTP call and reading the JSON — never block with `.Result`.
- A failed response (non-2xx) must produce `null`, not an exception and not a `WeatherResult` with empty/default fields.
