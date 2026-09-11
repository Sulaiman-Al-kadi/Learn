# Exercise — JSON file store

Write everything in `JsonFileStore.cs`.

## `Student` (record, already given)
```csharp
public record Student(string Name, int Age, List<int> Grades);
```

## `JsonFileStore` (static class)

| Method | Does |
|---|---|
| `string ToJson(List<Student> students)` | serialize the list to a JSON string |
| `List<Student> FromJson(string json)` | deserialize a JSON string back to a list; if the result would be `null`, return an **empty list** instead |
| `void SaveToFile(List<Student> students, string path)` | serialize, then write the JSON to `path` |
| `List<Student> LoadFromFile(string path)` | if `path` doesn't exist, return an **empty list** (don't throw); otherwise read it and deserialize |

## Rules
- Use `System.Text.Json.JsonSerializer` for both directions.
- `FromJson` and `LoadFromFile` must never return `null` — always a list (possibly empty).
- `LoadFromFile` must check `File.Exists` first — a missing file is a normal, expected case, not an error.
