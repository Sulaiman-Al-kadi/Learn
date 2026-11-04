## Hint 1
`File.AppendAllTextAsync(path, note + Environment.NewLine)` creates the file when it is missing.

## Hint 2
Check `File.Exists(path)` before `File.ReadAllLinesAsync(path)`.
