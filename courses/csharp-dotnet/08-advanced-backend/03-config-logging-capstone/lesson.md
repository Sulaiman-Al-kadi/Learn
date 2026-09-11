# Module 8 Capstone — Configuration & Structured Logging

Two last pieces, matching the university plan's Week 6 deliverable exactly: a background service with **configurable** settings, producing **structured JSON logs**.

## `appsettings.json` and `IConfiguration`

Every ASP.NET Core app already reads `appsettings.json` automatically:

```json
{
  "Notifier": {
    "IntervalMinutes": 5,
    "FromAddress": "noreply@example.com"
  }
}
```

```csharp
string? from = builder.Configuration["Notifier:FromAddress"];               // raw string access — works, but easy to typo the key
```

## The Options pattern — strongly-typed, validated configuration

Raw string-keyed access is fragile. The **Options pattern** binds a configuration section to a real C# class instead:

```csharp
public class NotifierSettings
{
    public int IntervalMinutes { get; set; } = 5;
    public string FromAddress { get; set; } = "";
}
```

```csharp
builder.Services.Configure<NotifierSettings>(builder.Configuration.GetSection("Notifier"));
```

Then anywhere in your app, inject `IOptions<NotifierSettings>` (Module 5's DI, one more injectable type) instead of manually reading strings:

```csharp
public class Notifier
{
    private readonly NotifierSettings _settings;

    public Notifier(IOptions<NotifierSettings> options)
    {
        _settings = options.Value;              // the bound, strongly-typed settings object
    }

    public string Describe() => $"Runs every {_settings.IntervalMinutes} minutes from {_settings.FromAddress}";
}
```

Typos in a JSON key now surface as "property just keeps its default value" rather than a silent, hard-to-trace bug from a mistyped string literal scattered across the codebase — and you get IntelliSense/autocomplete on `_settings.` instead of guessing configuration key strings.

## `ILogger<T>` — the abstraction you should code against

You've used `Console.WriteLine` for output since lesson 1 of this whole course. Real applications use a **logging abstraction** instead — `ILogger<T>`, injected like anything else:

```csharp
public class Notifier
{
    private readonly ILogger<Notifier> _logger;

    public Notifier(ILogger<Notifier> logger)
    {
        _logger = logger;
    }

    public void SendNotification(string to, string message)
    {
        _logger.LogInformation("Sending notification to {To}: {Message}", to, message);
    }
}
```

| Level | Use for |
|---|---|
| `LogTrace`/`LogDebug` | fine-grained detail, usually off in production |
| `LogInformation` | normal, expected events worth recording |
| `LogWarning` | something unexpected, but not broken |
| `LogError`/`LogCritical` | something failed |

Notice `{To}` and `{Message}` in the format string — these are **named placeholders**, not string interpolation. This distinction is the whole point of **structured logging** (next).

## Structured logging — logs as data, not just text

```csharp
_logger.LogInformation("Sending notification to {To}: {Message}", to, message);
```

This looks like `string.Format`, but it isn't — the logging provider keeps `To` and `Message` as **separate, named, queryable fields**, not just baked into one flat string. Serilog (the most popular structured logging library for .NET) can write this out as JSON:

```json
{ "Timestamp": "2026-01-01T10:00:00Z", "Level": "Information", "Message": "Sending notification to ali@example.com: Reminder", "To": "ali@example.com", "MessageText": "Reminder" }
```

Now you can search/filter logs by the actual field `To` or `Level`, in a log aggregation tool, instead of grep-ing through plain text — this is the "Structured JSON logs" deliverable the university plan asks for.

## Wiring up Serilog (what it looks like in `Program.cs`)

```csharp
using Serilog;

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .WriteTo.Console()
        .WriteTo.File(new Serilog.Formatting.Json.JsonFormatter(), "logs/app.json");
});
```

From this point on, every `_logger.LogInformation(...)` call anywhere in your app — including inside a `BackgroundService` — is written both to the console (human-readable) and to `logs/app.json` (structured, machine-readable), with zero changes to the code that actually calls `_logger.LogInformation(...)`. This is precisely why coding against `ILogger<T>` (the abstraction) rather than a specific logging library pays off — Module 2's DIP, once more.

## Bringing it together — the capstone's `Notifier`

The exercise combines everything: `NotifierSettings` bound via the Options pattern, and a `Notifier` class that logs structured, leveled messages using its configured settings — the same shape a `BackgroundService` (lesson 01) would call on each `PeriodicTimer` tick in a real scheduled-notification system.

## Summary
- `appsettings.json` + `IConfiguration` is the base configuration system; raw string-keyed access is fragile.
- The **Options pattern** (`Configure<T>(section)` + inject `IOptions<T>`) binds configuration to a strongly-typed class instead.
- `ILogger<T>` is the logging **abstraction** you code against — never tie your code directly to a specific logging library.
- **Structured logging**: `{Placeholder}` syntax in `Log*` calls keeps values as separate, named, queryable fields — not just interpolated text.
- Serilog wires up where those structured logs actually go (console, JSON file, ...) — configured once, used everywhere via the same `ILogger<T>` calls.
