using Xunit;

public class SettingsFileTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
    public void Dispose() { if (File.Exists(_path)) File.Delete(_path); }

    [Fact] public void Missing_GivesDefaults() => Assert.Equal(new Settings("light", 14), SettingsFile.Load(_path));
    [Fact] public void RoundTrip()
    {
        SettingsFile.Save(_path, new Settings("dark", 16));
        Assert.Equal(new Settings("dark", 16), SettingsFile.Load(_path));
    }
    [Fact] public void Saved_AsJson()
    {
        SettingsFile.Save(_path, new Settings("dark", 16));
        Assert.Contains("\"Theme\":\"dark\"", File.ReadAllText(_path));
    }
    [Fact] public void Update_ChangesAndSaves()
    {
        SettingsFile.Update(_path, s => s with { FontSize = 20 });
        Assert.Equal(new Settings("light", 20), SettingsFile.Load(_path));
    }
}
