## Hint 1
`await foreach (int n in reader.ReadAllAsync())` ends when the writer completes.

## Hint 2
`writer.Complete()` is what signals the end. Use `await writer.WriteAsync(n)` for each number.
