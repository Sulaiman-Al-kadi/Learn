# Exercise — NotificationQueue with Channel<T>

Write everything in `NotificationQueue.cs`.

## `Notification` (record)
```csharp
public record Notification(string To, string Message);
```

## `NotificationQueue`
Wraps a `Channel<Notification>` internally (`Channel.CreateUnbounded<Notification>()`).

| Method | Does |
|---|---|
| `Task EnqueueAsync(Notification notification)` | writes the notification to the channel |
| `void CompleteAdding()` | marks the channel's writer as complete — no more items will be added. **Required** before draining, otherwise `DrainAllAsync` would wait forever for more items that will never come. |
| `Task<List<Notification>> DrainAllAsync()` | reads every notification from the channel, in order, into a list, and returns it once the channel is complete |

## Rules
- Use `_channel.Writer.WriteAsync(...)`, `_channel.Writer.Complete()`, and `_channel.Reader.ReadAllAsync()` (with `await foreach`) exactly as shown in the lesson.
- `DrainAllAsync` must preserve the order notifications were enqueued in (FIFO).
