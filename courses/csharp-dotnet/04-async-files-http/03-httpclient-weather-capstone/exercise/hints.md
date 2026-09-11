## Hint 1
Store the client exactly like every other constructor-injection example this module: `private readonly HttpClient _http; public WeatherClient(HttpClient http) { _http = http; }`

## Hint 2
```csharp
public async Task<WeatherResult?> GetWeatherAsync(string city)
{
    HttpResponseMessage response = await _http.GetAsync($"/weather?city={city}");
    if (!response.IsSuccessStatusCode) return null;
    return await response.Content.ReadFromJsonAsync<WeatherResult>();
}
```

## Hint 3
Both the `GetAsync` call and `ReadFromJsonAsync` need `await` — they're both async operations. Forgetting `await` on `ReadFromJsonAsync` would give you a `Task<WeatherResult?>` instead of the actual `WeatherResult?`, which won't compile against the method's return statement.
