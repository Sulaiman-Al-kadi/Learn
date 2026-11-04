public class NoteLog
{
    private readonly string _path;

    public NoteLog(string path) => _path = path;

    public async Task AddAsync(string note)
    {
        await File.AppendAllTextAsync(_path, note + Environment.NewLine);
    }

    public async Task<List<string>> ReadAllAsync()
    {
        if (!File.Exists(_path)) return [];
        return (await File.ReadAllLinesAsync(_path)).ToList();
    }

    public async Task<int> CountAsync()
    {
        return (await ReadAllAsync()).Count;
    }
}
