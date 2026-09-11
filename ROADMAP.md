# Course Roadmap — every concept, module by module

This is the complete map of what `courses/csharp-dotnet` covers, start to finish: from your first `Console.WriteLine` to a deployed, tested, secured .NET backend. It mirrors the Islamic University of Madinah's 200-hour .NET Core training plan, expanded into 10 modules with hands-on, auto-checked exercises for nearly every concept.

**Status:** all 10 modules are built — 33 lessons, ~117 checkable items (lessons + quizzes + exercises), all exercises verified (solution passes, starter fails).

---

## Part I — The C# Language (Modules 1–4)

### Module 1 — C# Basics (9 lessons)
Printing & escape sequences · variables & the 5 core types (`int`, `double`, `string`, `bool`, `char`) · reading input & `TryParse`-style safety · operators & the integer-division trap · `if`/`else`/`else if` chains & the conditional operator · `while`/`do-while`/`for`/`foreach`, `break`/`continue` · methods, overloading, `params`, tuples, local functions · arrays vs. `List<T>`, reference semantics · string methods, `StringBuilder`, `int.TryParse`.
**Capstone-equivalent exercises:** receipt printer, student card, order calculator, grade statistics, cinema pricing, number stats + multiplication table, helper method library, list toolkit, text tools library.

### Module 2 — OOP & SOLID (10 lessons)
Classes & objects, constructors, `this` · encapsulation, properties (`get`/`set`/`init`), computed properties · inheritance, `virtual`/`override`, polymorphism, `base` · interfaces vs. abstract classes · records & value equality, `with` · generics, constraints, building a generic `Stack<T>` · exception handling, custom exceptions, guard clauses · **SOLID** (SRP, OCP, LSP, ISP, DIP) grounded in every prior lesson · writing unit tests with xUnit (`[Fact]`/`[Theory]`, AAA, `Assert.Throws`).
**Capstone:** Library lending system — SRP-separated classes, DIP-injected late-fee policy, custom exceptions, 20 passing tests.

### Module 3 — Collections & LINQ (3 lessons)
`Where`/`Select`/`OrderBy`/`ThenBy`, method syntax & lambdas · aggregates (`Sum`/`Average`/`Min`/`Max`/`Count`), `Any`/`All`, `First(OrDefault)`/`Single(OrDefault)`, `GroupBy`, `Skip`/`Take` · deferred execution, the multiple-enumeration trap, `Distinct`.
**Capstone:** Log Analyzer — filtering, grouping, pagination over parsed log entries.

### Module 4 — Async, Files, JSON & HTTP (3 lessons)
`async`/`await`, `Task`/`Task<T>`, `Task.WhenAll`, exceptions across `await`, `async void` pitfalls · file I/O, `System.Text.Json` serialize/deserialize, save/load patterns · `HttpClient`, `ReadFromJsonAsync`, DI-injected clients, testing HTTP code with a fake `HttpMessageHandler`.
**Capstone:** WeatherClient — a real-shaped API client, fully unit-testable without a live network call.

---

## Part II — .NET Backends (Modules 5–9)

### Module 5 — ASP.NET Core Web Fundamentals (3 lessons)
The request pipeline, custom middleware, `app.Use`/`next()`, ordering · route parameters & constraints, query strings, request-body binding, `Results.*` status codes · MVC controllers (`[ApiController]`, `[Route]`, `[HttpGet]`, `ControllerBase`) vs. Minimal APIs · Dependency Injection — registering & consuming services, `Singleton`/`Scoped`/`Transient` lifetimes.
**Capstone:** StudentsApi v1 — `IStudentRepository` + DI, tested with `WebApplicationFactory`.

### Module 6 — EF Core (3 lessons)
`DbContext`, `DbSet<T>`, CRUD via change tracking, migrations vs. `EnsureCreated` · one-to-many relationships, navigation properties, `Include` & the lazy-navigation trap, cascade delete.
**Capstone:** StudentsApi v2 — the same API, now on real SQLite via EF Core; integration tests swap in an in-memory SQLite connection.

### Module 7 — Minimal APIs & Security (3 lessons)
OpenAPI generation (`AddOpenApi`/`MapOpenApi`) & Swagger UI · JWT issuance & validation, `[Authorize]`/`RequireAuthorization`, `401` vs `403` · role-based policies, CORS (and what it does/doesn't protect).
**Capstone:** Role-secured TaskApi with CORS — full auth flow tested end-to-end.

### Module 8 — Advanced Backend Topics (3 lessons)
`BackgroundService`, `PeriodicTimer`, scoped services inside a singleton worker, `Channel<T>` producer/consumer · `IMemoryCache`, cache-aside, invalidation · configuration & the Options pattern (`IOptions<T>`), `ILogger<T>` & structured logging, Serilog JSON output.
**Capstone:** Notifier — Options-bound settings + structured, leveled logging, fully unit-tested via a fake `ILogger`.

### Module 9 — Testing, Docker & DevOps (3 lessons)
The testing pyramid (unit/integration/E2E) named against what you already wrote · contract testing with `JsonDocument` · **[guided lab]** Dockerfiles, multi-stage builds, `docker-compose` · **[guided lab]** GitHub Actions CI/CD, `needs:`-chained jobs, secrets, branch protection.
**Capstone:** contract-tested StudentsApi + a real pipeline you run yourself once Docker Desktop is installed.

---

## Part III — Capstone (Module 10)

Your own project (booking system, attendance dashboard, or similar scope) built with whatever mix of Modules 1–9 the scope actually needs. Deliverables: a working MVP, real persistence, integration tests, a README, real git/PR history, and a written learning report — matching the plan's final evaluation (project + 30-minute technical interview).

---

## How this maps to the university plan

| Plan week | Plan topic | Course module(s) |
|---|---|---|
| 1 | Environment, Git, C# syntax, memory/GC | Module 1 (memory/GC covered conceptually via value/reference semantics throughout; see also the earlier deleted W1 notes — reintroduce a dedicated memory lesson if you want the GC deep-dive back) |
| 2 | OOP, SOLID, unit testing, ≥80% coverage | Module 2 |
| 3 | Middleware, MVC vs Razor Pages, DI | Module 5 |
| 4 | EF Core, migrations, relationships | Module 6 |
| 5 | Minimal APIs, Swagger, JWT/OAuth2, CORS | Module 7 |
| 6 | Async/Channels, background workers, caching, logging, config | Module 8 (async itself is Module 4) |
| 7 | Integration/contract tests, Docker, pipeline | Module 9 |
| 8 | Capstone, sprints, code review | Module 10 |

`Module 3` (LINQ) isn't a separate week in the original plan — it's folded into "C# syntax" there, but broken out here because it's foundational to everything from Module 6 onward and deserves dedicated practice.

---

## What's intentionally out of scope

- **Razor Pages / server-rendered MVC views** — mentioned in Module 5 for recognition, not built; this course stays API-focused per your stated goal.
- **A real deployed cloud environment** (Azure/AWS) — Module 9's Docker/CI lessons prepare you for this, but actual cloud deployment needs an account and isn't part of the auto-checked content.
- **SQL Server / PostgreSQL specifics** — Module 6 uses SQLite throughout (zero setup); the EF Core code you write is nearly identical for other providers, just a different `Use...()` call.
- **Deep GC/memory internals** — the original from-scratch scaffold had a dedicated stack/heap/generational-GC lesson; it was removed in the pivot to Learn Lab. Ask if you want it rebuilt as a Module 1 addition.

If you finish all 10 modules and want to go further, the natural next steps are: gRPC, SignalR (real-time), a message broker (RabbitMQ/Azure Service Bus) replacing Module 8's in-process `Channel<T>`, and a real cloud deployment.
