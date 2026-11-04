using System.Net;
using System.Net.Http.Json;

public record Quote(int Id, string Text);

public class QuoteClient
{
    private readonly HttpClient _http;

    public QuoteClient(HttpClient http) => _http = http;

    public async Task<Quote?> GetAsync(int id)
    {
        throw new NotImplementedException();
    }
}
