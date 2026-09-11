## Hint 1
`EnqueueAsync`: `await _channel.Writer.WriteAsync(notification);`
`CompleteAdding`: `_channel.Writer.Complete();`

## Hint 2
```csharp
public async Task<List<Notification>> DrainAllAsync()
{
    var results = new List<Notification>();
    await foreach (Notification notification in _channel.Reader.ReadAllAsync())
    {
        results.Add(notification);
    }
    return results;
}
```

## Hint 3
`ReadAllAsync()` only finishes once the writer has been `Complete()`d — that's why the tests always call `CompleteAdding()` before `DrainAllAsync()`. Without it, the `await foreach` would wait forever for items that will never come.
