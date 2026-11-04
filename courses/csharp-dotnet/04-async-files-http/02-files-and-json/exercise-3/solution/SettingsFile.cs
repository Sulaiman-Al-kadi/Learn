using System.Text.Json;

public record Settings(string Theme, int FontSize);

public static class SettingsFile
{
    public static void Save(string path, Settings settings)
    {
        File.WriteAllText(path, JsonSerializer.Serialize(settings));
    }

    public static Settings Load(string path)
    {
        if (!File.Exists(path)) return new Settings("light", 14);
        return JsonSerializer.Deserialize<Settings>(File.ReadAllText(path))!;
    }

    public static Settings Update(string path, Func<Settings, Settings> change)
    {
        Settings updated = change(Load(path));
        Save(path, updated);
        return updated;
    }
}
