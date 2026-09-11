## Hint 1
Constructor: `_settings = options.Value; _logger = logger;` — note `options.Value`, not storing the `IOptions<NotifierSettings>` wrapper itself.

## Hint 2
```csharp
public void SendNotification(string to, string message)
{
    if (string.IsNullOrWhiteSpace(_settings.FromAddress))
    {
        _logger.LogWarning("FromAddress is not configured — cannot send");
        return;
    }
    _logger.LogInformation("Sending notification from {From} to {To}: {Message}", _settings.FromAddress, to, message);
}
```

## Hint 3
`Describe`: `return $"Notifier configured to run every {_settings.IntervalMinutes} minutes from {_settings.FromAddress}";`
