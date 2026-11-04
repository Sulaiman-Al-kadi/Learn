public class NoteLog
{
    private readonly string _path;

    public NoteLog(string path) => _path = path;

    public async Task AddAsync(string note)
    {
        throw new NotImplementedException();
    }

    public async Task<List<string>> ReadAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<int> CountAsync()
    {
        throw new NotImplementedException();
    }
}
