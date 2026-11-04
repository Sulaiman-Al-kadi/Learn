using Xunit;

public class NoteLogTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
    public void Dispose() { if (File.Exists(_path)) File.Delete(_path); }

    [Fact] public async Task Missing_IsEmpty() => Assert.Empty(await new NoteLog(_path).ReadAllAsync());
    [Fact] public async Task Add_KeepsOrder()
    {
        var log = new NoteLog(_path);
        await log.AddAsync("first");
        await log.AddAsync("second");
        Assert.Equal(["first", "second"], await log.ReadAllAsync());
    }
    [Fact] public async Task Survives_NewInstance()
    {
        await new NoteLog(_path).AddAsync("kept");
        Assert.Equal(1, await new NoteLog(_path).CountAsync());
    }
}
