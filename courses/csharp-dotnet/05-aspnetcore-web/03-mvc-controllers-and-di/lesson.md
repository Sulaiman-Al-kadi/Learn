# Module 5 Capstone — MVC Controllers & Dependency Injection

Two more pieces, then everything in this module comes together: the **other** way to define endpoints (MVC controllers, alongside lesson 01-02's Minimal APIs), and **Dependency Injection** — the container that supplies your classes with what they need, which is how real ASP.NET Core apps are structured.

## MVC controllers — the class-based alternative to Minimal APIs

```csharp
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(new[] { "Ali", "Sara" });
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        if (id != 1) return NotFound();
        return Ok("Ali");
    }
}
```

| Piece | Meaning |
|---|---|
| `[ApiController]` | Marks this as an API controller — enables some conveniences (automatic 400 responses for invalid model binding, among others). |
| `[Route("api/[controller]")]` | The base route. `[controller]` is replaced with the class name minus "Controller" — so `StudentsController` serves `api/students`. |
| `: ControllerBase` | The base class giving you `Ok()`, `NotFound()`, `Created(...)`, etc. — the same results as Minimal API's `Results.*`, just as inherited methods (Module 2, lesson 03!) instead of static calls. |
| `[HttpGet]`, `[HttpPost]`, ... | Marks a method as handling that HTTP verb. Combined with the class route: `[HttpGet("{id:int}")]` on `StudentsController` serves `GET api/students/{id}`. |
| `IActionResult` | The return type for an action that returns varying kinds of responses (`Ok`, `NotFound`, ...). |

Every endpoint here is a normal C# **method** on a normal C# **class** — everything from Module 2 (methods, classes, inheritance from `ControllerBase`) applies directly. This is genuinely just OOP, wearing a web framework's clothes.

## Minimal APIs vs. MVC controllers — when to use which

| | Minimal APIs (lessons 01-02) | MVC Controllers (this lesson) |
|---|---|---|
| Shape | Functions (`app.MapGet(...)`) | Classes with methods (`[HttpGet]`) |
| Best for | Small APIs, microservices, simple endpoint sets | Larger APIs, when you want attributes, filters, more structure |
| Grouping | Manual (you organize `Program.cs` yourself) | Automatic — one controller class per resource |

Both produce the exact same kind of HTTP API underneath — this course uses Minimal APIs for most exercises (less ceremony while learning), but recognize MVC controllers; many real, especially larger, .NET codebases use them, and the university plan explicitly names MVC as a Week 3 topic.

*(Razor Pages and full MVC Views — server-rendered HTML pages — are the other half of "MVC" you'll see mentioned elsewhere. This course stays API-focused, since your stated goal is backend/API work, but know they exist for building traditional server-rendered web UIs instead of JSON APIs.)*

## Dependency Injection — the container

Module 2, lesson 08 introduced DIP: depend on interfaces, get concrete implementations handed to you through a constructor. ASP.NET Core has a built-in **DI container** that does exactly this, automatically, for your whole app.

```csharp
public interface IStudentRepository
{
    List<string> GetAll();
}

public class InMemoryStudentRepository : IStudentRepository
{
    private readonly List<string> _students = ["Ali", "Sara"];
    public List<string> GetAll() => _students;
}
```

**Registering** a service — telling the container "when something asks for `IStudentRepository`, give them this":

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IStudentRepository, InMemoryStudentRepository>();
var app = builder.Build();
```

**Consuming** it — a controller (or a Minimal API handler) just asks for the interface, and the container supplies it:

```csharp
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentRepository _repository;

    public StudentsController(IStudentRepository repository)   // constructor injection — Module 2, lesson 08!
    {
        _repository = repository;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_repository.GetAll());
}
```

You never write `new InMemoryStudentRepository()` anywhere — the container resolves it. Minimal API handlers can request services the same way, as extra parameters:

```csharp
app.MapGet("/students", (IStudentRepository repository) => repository.GetAll());
```

## Service lifetimes

```csharp
builder.Services.AddSingleton<IStudentRepository, InMemoryStudentRepository>();   // ONE instance, shared for the app's whole lifetime
builder.Services.AddScoped<IOrderService, OrderService>();                          // ONE instance PER REQUEST
builder.Services.AddTransient<IEmailFormatter, EmailFormatter>();                    // a NEW instance every time it's requested
```

| Lifetime | One instance per... | Typical use |
|---|---|---|
| `Singleton` | the whole application | in-memory caches/stores, configuration, stateless helpers |
| `Scoped` | one HTTP request | anything touching a database connection (Module 6!) — consistent state across one request |
| `Transient` | every single injection | lightweight, stateless, cheap-to-create services |

Getting this wrong (e.g., a `Singleton` accidentally holding per-request state) is a classic real-world bug — for this module's in-memory storage, `Singleton` is exactly right (one shared list, for the app's lifetime).

## Why this matters

Every module from here on builds on DI: EF Core's `DbContext` (Module 6) is registered and injected exactly this way; JWT authentication services (Module 7); background workers and caching (Module 8) — all of it flows through the same container and the same constructor-injection pattern you just used for a simple in-memory repository.

## Summary
- MVC controllers: classes inheriting `ControllerBase`, `[HttpGet]`/`[HttpPost]`/... methods, `[Route]` for the path template — the same endpoints as Minimal APIs, expressed as OOP.
- Register a service: `builder.Services.AddSingleton<IInterface, Implementation>()` (or `AddScoped`/`AddTransient`).
- Consume it via constructor injection (controllers) or as an extra handler parameter (Minimal APIs) — never `new` it yourself.
- `Singleton` (whole app), `Scoped` (per request), `Transient` (every injection) — pick based on whether/how the service holds state.

## The capstone

Open the exercise. You'll build a `StudentsApi` — an `IStudentRepository` interface, an in-memory implementation registered as a singleton, and a `StudentsController` that depends on the interface, not the concrete class. This is Module 5's version of Module 2's Library System capstone: the same SOLID principles, now wired into a real running web app.
