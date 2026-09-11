// Capstone — WeatherClient
// See README.md.
using System.Net.Http.Json;

public record WeatherResult(string City, double TempC, string Condition);

public class WeatherClient
{
    // TODO: private readonly HttpClient field, set via the constructor

    public WeatherClient(HttpClient http)
    {
        throw new NotImplementedException();
    }

    public async Task<WeatherResult?> GetWeatherAsync(string city)
    {
        throw new NotImplementedException();
    }
}
