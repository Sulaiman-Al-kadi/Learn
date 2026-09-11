// Capstone — WeatherClient (solution)
using System.Net.Http.Json;

public record WeatherResult(string City, double TempC, string Condition);

public class WeatherClient
{
    private readonly HttpClient _http;

    public WeatherClient(HttpClient http)
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
