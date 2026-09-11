## Hint 1
`ToJson`: `return JsonSerializer.Serialize(students);`
`FromJson`: `return JsonSerializer.Deserialize<List<Student>>(json) ?? new List<Student>();` — the `??` handles the "deserialized to null" case.

## Hint 2
`SaveToFile`: `File.WriteAllText(path, ToJson(students));` — reuse the method you already wrote instead of calling Serialize again.

## Hint 3
`LoadFromFile`:
```csharp
if (!File.Exists(path)) return new List<Student>();
return FromJson(File.ReadAllText(path));
```
Again, reuse `FromJson` rather than duplicating the deserialize call.
