# Capstone — Notifier (Options + structured logging)

Write everything in `Notifier.cs`.

## `NotifierSettings`
```csharp
public class NotifierSettings
{
    public int IntervalMinutes { get; set; } = 5;
    public string FromAddress { get; set; } = "";
}
```

## `Notifier`
Constructor takes `IOptions<NotifierSettings>` and `ILogger<Notifier>` (both injected — store `options.Value`, not the `IOptions<...>` wrapper itself).

| Method | Does |
|---|---|
| `void SendNotification(string to, string message)` | if `_settings.FromAddress` is null/whitespace, `_logger.LogWarning("FromAddress is not configured — cannot send")` and **stop** (don't log anything else). Otherwise, `_logger.LogInformation("Sending notification from {From} to {To}: {Message}", _settings.FromAddress, to, message)`. |
| `string Describe()` | returns exactly `$"Notifier configured to run every {_settings.IntervalMinutes} minutes from {_settings.FromAddress}"` |

## Rules
- Use the **structured** logging placeholders (`{From}`, `{To}`, `{Message}`) exactly as shown — not string interpolation — this is the whole point of the lesson.
- `SendNotification` must check for a missing `FromAddress` **first**, before attempting to log the informational "sending" message.
