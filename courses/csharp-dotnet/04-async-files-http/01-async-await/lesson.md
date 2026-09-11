# Module 4, Lesson 01 — Async & Await

Everything you've written so far runs **synchronously** — one line waits for the previous one to fully finish. That's fine for math and loops, but terrible for anything that waits on something slow: a network request, a database query, reading a large file. This lesson is about not freezing your program while you wait.

## The problem

```csharp
string data = DownloadReport();   // takes 3 seconds
Console.WriteLine("Done");
```

While `DownloadReport()` runs, the **entire program** is frozen — nothing else happens for 3 seconds, even though the CPU is doing essentially nothing (it's just waiting for the network). In a desktop app this freezes the UI; in a server handling many users at once (Module 5 onward), it means one slow request blocks the thread from serving anyone else.

## `Task` — a value that isn't ready yet

```csharp
public Task<string> DownloadReportAsync()
{
    // ...
}
```

A `Task<T>` represents "a `T` that will exist eventually." A plain `Task` (no `<T>`) represents "an operation that will complete eventually, with no result value" — the async equivalent of `void`.

## `async` and `await`

```csharp
public async Task<string> DownloadReportAsync()
{
    Console.WriteLine("Starting download...");
    await Task.Delay(3000);                 // simulates 3 seconds of waiting, without freezing the thread
    Console.WriteLine("Download complete");
    return "report data";
}
```

| Piece | Meaning |
|---|---|
| `async` | Marks a method as containing `await`. It doesn't make anything run in parallel by itself — it enables the `await` keyword inside. |
| `Task<string>` | The return type — "eventually produces a `string`." |
| `await` | Pauses **this method** until the awaited operation finishes, **without blocking the thread**. Control returns to whoever called this method, which can go do other work meanwhile. |

Calling it:

```csharp
string report = await DownloadReportAsync();
Console.WriteLine(report);
```

`await` unwraps the `Task<string>` into a plain `string` once it's ready — you work with the *result*, not the `Task` wrapper, once you've awaited it.

## The rule: `async` methods return `Task` or `Task<T>`, and you `await` calls to them

```csharp
static async Task Main()          // Main itself can be async (top-level statements handle this for you automatically)
{
    string report = await DownloadReportAsync();
    Console.WriteLine(report);
}
```

Convention: name async methods with an `Async` suffix (`DownloadReportAsync`, `GetWeatherAsync`) so callers immediately know to `await` them.

**"Async goes all the way up."** If method A `await`s method B, A itself normally needs to be `async Task` (or `async Task<T>`) too — you can't casually call an async method without either awaiting it or explicitly deciding not to (which is usually a mistake — see below).

## What actually happens — an analogy

Ordering food at a counter:
- **Synchronous**: order, then stand at the counter doing nothing until food is ready, then step aside.
- **Asynchronous**: order, take a number, sit down and do something else; when your number's called (the `await` "wakes up"), you go get your food.

The waiting itself doesn't require your full attention — `await` lets the program do other useful work (or, in a UI, stay responsive; in a server, handle other requests) during that wait, then resumes exactly where it left off once the awaited thing completes.

## Running multiple things concurrently — `Task.WhenAll`

```csharp
async Task<List<string>> DownloadAllAsync()
{
    Task<string> t1 = DownloadReportAsync("A");
    Task<string> t2 = DownloadReportAsync("B");
    Task<string> t3 = DownloadReportAsync("C");

    string[] results = await Task.WhenAll(t1, t2, t3);    // waits for all three, running concurrently
    return results.ToList();
}
```

Calling the async methods **without** `await` starts each one immediately; they now run concurrently. `await Task.WhenAll(...)` then waits for all of them to finish together — much faster than `await`ing each one in sequence (three separate waits, one after another) when they don't depend on each other.

## Exceptions in async code

`try`/`catch` (Module 2, lesson 07) works exactly the same across an `await`:

```csharp
try
{
    string report = await DownloadReportAsync();
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Download failed: {ex.Message}");
}
```

An exception thrown inside an `async` method surfaces at the `await` call site, just like a normal exception would — you don't need any special async-specific handling.

## Don't do this: `async void` and blocking on async code

```csharp
async void DoSomething() { ... }         // AVOID — exceptions here can crash the whole program uncatchably

string result = DownloadReportAsync().Result;   // AVOID — blocks the thread waiting, defeating the entire point
```

`async void` is only acceptable for top-level UI event handlers (rare, and outside this course's scope) — everywhere else, use `async Task`. `.Result` or `.Wait()` on a `Task` blocks synchronously and can even deadlock in some environments — always `await` instead.

## `Task.Delay` vs `Thread.Sleep`

```csharp
await Task.Delay(1000);     // pauses THIS async operation, thread is free to do other work
Thread.Sleep(1000);          // blocks the ENTIRE thread — nothing else can happen, defeats the purpose of async
```

Never mix `Thread.Sleep` into async code — it reintroduces the exact freezing problem `async`/`await` exists to solve.

## Summary
- `Task` / `Task<T>` represent work that completes **eventually**.
- `async` marks a method as being able to `await`; `await` pauses that method (without blocking the thread) until the awaited operation finishes.
- Async methods return `Task`/`Task<T>` and are conventionally named with an `Async` suffix.
- `Task.WhenAll(...)` runs several independent async operations concurrently and waits for all of them.
- `try`/`catch` works normally across `await`.
- Avoid `async void` and blocking with `.Result`/`.Wait()` — always `await`.
- `Task.Delay` (async-friendly) vs `Thread.Sleep` (blocks the thread — don't use in async code).
