# Module 5, Lesson 01 — Middleware & the Request Pipeline

Everything up to now ran once, from the top, and finished. A **web server** is different: it starts once, then handles request after request, potentially thousands, for as long as it runs. This lesson is about how ASP.NET Core turns an incoming HTTP request into a response.

## Your first web app

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/hello", () => "Hello, World!");

app.Run();
```

| Piece | Meaning |
|---|---|
| `WebApplication.CreateBuilder(args)` | Sets up configuration, logging, and dependency injection (lesson 04) — the app's foundation. |
| `builder.Build()` | Produces the actual `app` — the running web application. |
| `app.MapGet("/hello", () => "Hello, World!")` | Registers an **endpoint**: when a `GET` request arrives for `/hello`, run this handler and send its return value back as the response. This is a **Minimal API** — the modern, lightweight way to define endpoints (you'll see MVC controllers, the other way, in lesson 03). |
| `app.Run()` | Starts listening for requests. This blocks (like `Console.ReadLine()` blocking for input) — the program stays alive, serving requests, until you stop it. |

Run it (`dotnet run`) and visit `http://localhost:5000/hello` (the exact port is printed in the console) — you'll see `Hello, World!`.

## The request pipeline — middleware

Before your endpoint even runs, the request passes through a chain of **middleware** — pieces of code that each get a chance to inspect, modify, log, short-circuit, or pass along the request. Picture it as a series of checkpoints:

```
Request  →  [Middleware 1]  →  [Middleware 2]  →  [Middleware 3]  →  Endpoint handler
                                                                           ↓
Response ←  [Middleware 1]  ←  [Middleware 2]  ←  [Middleware 3]  ←──────┘
```

Each middleware runs code **before** passing control to the next one, and can run more code **after** the next one returns — that's why the diagram shows the response flowing back out through the same chain, in reverse.

## Writing your own middleware

```csharp
app.Use(async (context, next) =>
{
    Console.WriteLine($"→ {context.Request.Method} {context.Request.Path}");
    await next();                                    // pass control to the next middleware / the endpoint
    Console.WriteLine($"← {context.Response.StatusCode}");
});

app.MapGet("/hello", () => "Hello, World!");

app.Run();
```

| Piece | Meaning |
|---|---|
| `app.Use(async (context, next) => { ... })` | Registers a middleware. `context` (an `HttpContext`) gives you the request and response; `next` is a delegate for "continue to the next thing in the pipeline." |
| `await next();` | This is the crucial line — **everything after it** runs after the rest of the pipeline (including your endpoint) has finished. Everything **before** it runs first, before the request even reaches your endpoint. |

If you never call `next()`, the request stops right there — nothing further down the pipeline (including your actual endpoint) ever runs. This is called **short-circuiting**, and it's exactly how things like authentication middleware work: "if not logged in, respond with 401 and stop; otherwise, `next()`."

## Order matters — a lot

```csharp
app.Use(async (context, next) => { Console.WriteLine("A"); await next(); Console.WriteLine("A done"); });
app.Use(async (context, next) => { Console.WriteLine("B"); await next(); Console.WriteLine("B done"); });
app.MapGet("/", () => "Hi");
```

Visiting `/` prints:
```
A
B
Hi (handled)
B done
A done
```

Middleware registered first runs its "before" code first — but its "after" code runs **last**, because it's the outermost wrapper. This nested structure is why the order you call `app.Use(...)` in matters enormously; putting authentication middleware after your endpoints, for instance, would mean it never gets a chance to block anything.

## Built-in middleware

Real apps mostly use middleware ASP.NET Core already provides, added the same way:

```csharp
app.UseHttpsRedirection();    // redirect HTTP requests to HTTPS
app.UseStaticFiles();          // serve files from wwwroot/ directly
app.UseRouting();               // figure out which endpoint matches the request's path
app.UseAuthentication();        // who is this? (Module 7)
app.UseAuthorization();          // are they allowed to do this? (Module 7)
```

You'll add these as later modules need them — for now, know that `app.MapGet(...)` and friends implicitly participate in routing without you needing to call `UseRouting()` explicitly in simple minimal API apps (the framework wires it up for you).

## `app.Map*` — the endpoints themselves

```csharp
app.MapGet("/students", () => new[] { "Ali", "Sara" });
app.MapPost("/students", (string name) => $"Added {name}");
app.MapPut("/students/{id}", (int id) => $"Updated {id}");
app.MapDelete("/students/{id}", (int id) => $"Deleted {id}");
```

Each corresponds to an HTTP **method** — `GET` (read), `POST` (create), `PUT` (replace/update), `DELETE` (remove). This is the vocabulary of REST APIs, which the rest of this module and Module 7 build on directly.

## Returning different kinds of responses

```csharp
app.MapGet("/text", () => "plain text");                             // 200 OK, text/plain
app.MapGet("/data", () => new { Name = "Ali", Age = 20 });             // 200 OK, serialized to JSON automatically
app.MapGet("/missing", () => Results.NotFound());                       // 404
app.MapGet("/created", () => Results.Created("/items/1", new { Id = 1 })); // 201
app.MapGet("/bad", () => Results.BadRequest("Invalid input"));           // 400
```

Returning a plain object automatically serializes it to JSON (Module 4, lesson 02's `System.Text.Json`, working behind the scenes) — you rarely call `JsonSerializer.Serialize` yourself in a web app; the framework does it for you as part of the response pipeline.

## Testing a web app without a real browser

You can't easily "type an answer" into a running server the way earlier exercises fed `input.txt`. Instead, this module's exercises use `WebApplicationFactory` — a tool that spins up your entire app **in memory** and lets a test send it real HTTP requests, exactly like a browser or `HttpClient` would (Module 4, lesson 03!), just without a real network or port involved:

```csharp
var factory = new WebApplicationFactory<Program>();
var client = factory.CreateClient();

var response = await client.GetAsync("/hello");
string body = await response.Content.ReadAsStringAsync();
```

This is genuinely how professional ASP.NET Core projects are tested — you're learning the real technique, not a simplified stand-in.

## Summary
- `WebApplication.CreateBuilder(args)` → `builder.Build()` → `app.Run()` is the skeleton of every ASP.NET Core app.
- Requests flow through **middleware** in the order registered; `await next()` passes control onward, and code after it runs on the way back out.
- Not calling `next()` short-circuits the pipeline.
- `app.MapGet/MapPost/MapPut/MapDelete(path, handler)` define **Minimal API** endpoints.
- Returned objects are automatically serialized to JSON; `Results.NotFound()`, `Results.Created(...)`, etc. produce specific status codes.
- `WebApplicationFactory` runs your whole app in memory for testing, without a real server or network.
