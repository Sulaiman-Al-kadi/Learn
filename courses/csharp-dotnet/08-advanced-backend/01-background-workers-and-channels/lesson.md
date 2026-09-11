# Module 8, Lesson 01 — Background Workers & Channels

Everything so far responds to a request and finishes. Real applications also need work that runs **continuously in the background**, independent of any single HTTP request: sending scheduled notifications, processing a queue, cleaning up old data every hour. This lesson covers how.

## `BackgroundService` — long-running work tied to your app's lifetime

```csharp
using Microsoft.Extensions.Hosting;

public class HeartbeatWorker : BackgroundService
{
    private readonly ILogger<HeartbeatWorker> _logger;

    public HeartbeatWorker(ILogger<HeartbeatWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Still alive at {Time}", DateTimeOffset.UtcNow);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
```

| Piece | Meaning |
|---|---|
| `: BackgroundService` | Inheritance (Module 2!) — gives you the plumbing to run alongside your app's normal request handling. |
| `ExecuteAsync(CancellationToken stoppingToken)` | Your actual work. Runs once, for the entire lifetime of the app — usually structured as a loop. |
| `stoppingToken` | Signaled when the app is shutting down — check it in your loop condition, and pass it into any `await` so long-running work stops promptly instead of blocking shutdown. |

Registering it (Module 5's DI, one more service type):

```csharp
builder.Services.AddHostedService<HeartbeatWorker>();
```

From then on, `HeartbeatWorker` starts when the app starts and runs until the app shuts down — completely independent of any individual HTTP request.

## `PeriodicTimer` — scheduled, recurring work

`Task.Delay` in a loop works, but `PeriodicTimer` is the purpose-built tool for "run this every N seconds":

```csharp
public class NotificationWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await SendPendingNotificationsAsync();
        }
    }

    private Task SendPendingNotificationsAsync() => Task.CompletedTask;    // your real logic here
}
```

`WaitForNextTickAsync` asynchronously waits until the next interval, returning `false` (ending the loop cleanly) when the token is cancelled — slightly more precise and purpose-built than manually re-checking `IsCancellationRequested` after every `Task.Delay`.

## Scoped services inside a `BackgroundService`

`BackgroundService` implementations are typically registered as singletons (one instance for the app's lifetime — Module 5's lifetime table), but they often need `Scoped` services (like a `DbContext`, Module 6) which aren't safe to hold onto long-term. The fix: create a new scope each time you need one:

```csharp
public class CleanupWorker : BackgroundService
{
    private readonly IServiceProvider _services;

    public CleanupWorker(IServiceProvider services)
    {
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // ... use context, then let the scope (and context) be disposed ...
        }
    }
}
```

This pattern — inject `IServiceProvider` itself, then `CreateScope()` per unit of work — is exactly how a `BackgroundService` safely uses request-scoped services without a real HTTP request to scope them to.

## `Channel<T>` — a thread-safe producer/consumer queue

Sometimes one part of your app **produces** work items and another part **consumes** them, at different paces — an endpoint queues a notification to send; a background worker actually sends it, without making the original request wait. `Channel<T>` (from `System.Threading.Channels`) is an async-friendly queue built for exactly this:

```csharp
using System.Threading.Channels;

Channel<string> channel = Channel.CreateUnbounded<string>();

// Producer — e.g. called from an API endpoint
await channel.Writer.WriteAsync("Send welcome email to ali@example.com");

// Consumer — e.g. a BackgroundService's ExecuteAsync
await foreach (string message in channel.Reader.ReadAllAsync(stoppingToken))
{
    Console.WriteLine($"Processing: {message}");
}
```

| Piece | Meaning |
|---|---|
| `Channel.CreateUnbounded<T>()` | A queue with no fixed capacity (also: `CreateBounded<T>(capacity)` to cap how much can be queued at once, applying backpressure). |
| `channel.Writer.WriteAsync(item)` | Add an item — safe to call from multiple places concurrently. |
| `channel.Reader.ReadAllAsync()` | An `async` stream of items as they arrive — `await foreach` (Module 4's async, extended to a sequence) processes them one at a time as they're produced. |

This decouples **when work is requested** from **when it's done** — an API endpoint can respond immediately after queuing work, while a background worker processes the queue at its own pace. It's the in-process, lightweight cousin of a full message queue (RabbitMQ, Azure Service Bus) you might reach for in a larger system.

## Summary
- `BackgroundService` (`: BackgroundService`, override `ExecuteAsync`) runs work for the app's whole lifetime, registered with `AddHostedService<T>()`.
- Always respect `stoppingToken` — check it in loop conditions and pass it to `await`s so shutdown isn't blocked.
- `PeriodicTimer` + `WaitForNextTickAsync` is the purpose-built tool for "run this every N minutes."
- A long-lived `BackgroundService` needs `IServiceProvider.CreateScope()` to safely use `Scoped` services like a `DbContext`.
- `Channel<T>` is a thread-safe, async-friendly producer/consumer queue — decouple "request the work" from "do the work," typically an endpoint writing and a `BackgroundService` reading via `await foreach`.
