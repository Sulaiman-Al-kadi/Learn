// Exercise 02 — JSON file store
// See README.md.
using System.Text.Json;

public record Student(string Name, int Age, List<int> Grades);

public static class JsonFileStore
{
    public static string ToJson(List<Student> students)
    {
        throw new NotImplementedException();
    }

    public static List<Student> FromJson(string json)
    {
        throw new NotImplementedException();
    }

    public static void SaveToFile(List<Student> students, string path)
    {
        throw new NotImplementedException();
    }

    public static List<Student> LoadFromFile(string path)
    {
        throw new NotImplementedException();
    }
}
