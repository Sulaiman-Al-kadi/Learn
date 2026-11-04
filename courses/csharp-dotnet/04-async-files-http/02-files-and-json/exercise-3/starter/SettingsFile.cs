using System.Text.Json;

public record Settings(string Theme, int FontSize);

public static class SettingsFile
{
    public static void Save(string path, Settings settings)
    {
        throw new NotImplementedException();
    }

    public static Settings Load(string path)
    {
        throw new NotImplementedException();
    }

    public static Settings Update(string path, Func<Settings, Settings> change)
    {
        throw new NotImplementedException();
    }
}
