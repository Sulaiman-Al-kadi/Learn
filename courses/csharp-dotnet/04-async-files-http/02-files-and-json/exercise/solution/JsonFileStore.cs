// Exercise 02 — JSON file store (solution)
using System.Text.Json;

public record Student(string Name, int Age, List<int> Grades);

public static class JsonFileStore
{
    public static string ToJson(List<Student> students)
    {
        return JsonSerializer.Serialize(students);
    }

    public static List<Student> FromJson(string json)
    {
        return JsonSerializer.Deserialize<List<Student>>(json) ?? new List<Student>();
    }

    public static void SaveToFile(List<Student> students, string path)
    {
        File.WriteAllText(path, ToJson(students));
    }

    public static List<Student> LoadFromFile(string path)
    {
        if (!File.Exists(path)) return new List<Student>();
        return FromJson(File.ReadAllText(path));
    }
}
