# Module 4, Lesson 02 — Files & JSON

Two practical skills that combine constantly: reading/writing files, and converting between C# objects and JSON — the text format nearly every API (Module 5 onward) speaks.

## Reading and writing whole files

```csharp
using System.IO;

// Write
File.WriteAllText("notes.txt", "Hello, file!");

// Read
string content = File.ReadAllText("notes.txt");
Console.WriteLine(content);

// Append
File.AppendAllText("notes.txt", "\nAnother line");

// Line by line
string[] lines = File.ReadAllLines("notes.txt");
foreach (string line in lines) Console.WriteLine(line);

// Does it exist?
if (File.Exists("notes.txt")) { /* ... */ }

File.Delete("notes.txt");
```

These are **synchronous** by default. .NET also provides async versions for larger files or when responsiveness matters (Module 4, lesson 01!):

```csharp
string content = await File.ReadAllTextAsync("notes.txt");
await File.WriteAllTextAsync("notes.txt", "Hello, file!");
```

Same idea, same naming convention (`Async` suffix, returns `Task`) as every other async method you've seen.

## Paths — don't hardcode them

```csharp
string folder = "data";
string fullPath = Path.Combine(folder, "notes.txt");   // "data/notes.txt" or "data\notes.txt" — correct for the OS
Directory.CreateDirectory(folder);                        // creates it if it doesn't already exist; no error if it does
```

`Path.Combine` avoids manually gluing strings with `/` or `\` — it uses the right separator for whatever OS the program runs on (this course happens to be on Windows, but well-written C# doesn't assume that).

## JSON — the universal data format

JSON (JavaScript Object Notation) represents structured data as text:
```json
{
  "name": "Sara",
  "age": 25,
  "isStudent": false,
  "grades": [88, 92, 79]
}
```
Objects (`{ }`) map to your classes/records; arrays (`[ ]`) map to lists; the primitive types map directly to C#'s `string`, numbers, `bool`.

## `System.Text.Json` — serializing (C# object → JSON text)

```csharp
using System.Text.Json;

public record Student(string Name, int Age, bool IsStudent, List<int> Grades);

var student = new Student("Sara", 25, false, [88, 92, 79]);

string json = JsonSerializer.Serialize(student);
Console.WriteLine(json);
// {"Name":"Sara","Age":25,"IsStudent":false,"Grades":[88,92,79]}
```

**Serializing** = turning a C# object into JSON text. One call: `JsonSerializer.Serialize(obj)`.

## Deserializing (JSON text → C# object)

```csharp
string json = """{"Name":"Ali","Age":30,"IsStudent":true,"Grades":[70,80]}""";

Student? student = JsonSerializer.Deserialize<Student>(json);
Console.WriteLine(student?.Name);   // Ali
```

**Deserializing** = parsing JSON text back into a real object. Note `Deserialize<Student>` — a **generic method** (Module 2, lesson 06!) where `T` tells it what shape to build. The result is nullable (`Student?`) because the JSON might not actually match — always account for that.

The property **names** in the JSON must line up with your type's property names (case-insensitively, by default) — `System.Text.Json` matches `"Name"` in the JSON to the `Name` property on `Student` automatically.

## Pretty-printing

```csharp
var options = new JsonSerializerOptions { WriteIndented = true };
string prettyJson = JsonSerializer.Serialize(student, options);
```
```json
{
  "Name": "Sara",
  "Age": 25,
  "IsStudent": false,
  "Grades": [
    88,
    92,
    79
  ]
}
```

## Combining files and JSON — saving/loading objects

This pattern — persisting real data to disk between runs — is extremely common before you have a real database (Module 6):

```csharp
public static class StudentStore
{
    private const string FilePath = "students.json";

    public static void Save(List<Student> students)
    {
        string json = JsonSerializer.Serialize(students, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }

    public static List<Student> Load()
    {
        if (!File.Exists(FilePath)) return [];
        string json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<List<Student>>(json) ?? [];
    }
}
```

Notice `Deserialize<List<Student>>` — generics compose naturally: "deserialize into a `List<Student>`," the same way `List<int>` and `List<string>` are both valid.

## Handling bad/missing JSON

```csharp
try
{
    var student = JsonSerializer.Deserialize<Student>(possiblyBadJson);
}
catch (JsonException ex)
{
    Console.WriteLine($"Invalid JSON: {ex.Message}");
}
```
Malformed JSON throws `JsonException` — same `try`/`catch` pattern as everywhere else (Module 2, lesson 07).

## Summary
- `File.ReadAllText`/`WriteAllText`/`AppendAllText`/`Exists`/`Delete`, plus `...Async` versions for larger files.
- `Path.Combine(...)` builds OS-correct paths; `Directory.CreateDirectory(...)` ensures a folder exists.
- `JsonSerializer.Serialize(obj)` — C# object → JSON string.
- `JsonSerializer.Deserialize<T>(json)` — JSON string → C# object (nullable result; a generic method).
- `JsonSerializerOptions { WriteIndented = true }` for readable, pretty-printed output.
- Save/load patterns combine both: serialize to a string, write it to a file; read the file, deserialize back.
- Malformed JSON throws `JsonException` — handle it like any other exception.
