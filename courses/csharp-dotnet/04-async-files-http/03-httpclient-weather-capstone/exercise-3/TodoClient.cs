using System.Net.Http.Json;

public record NewTodo(string Title, bool Done);
public record Todo(int Id, string Title, bool Done);

public class TodoClient
{
    private readonly HttpClient _http;

    public TodoClient(HttpClient http) => _http = http;

    public async Task<Todo> CreateAsync(string title)
    {
        throw new NotImplementedException();
    }
}
