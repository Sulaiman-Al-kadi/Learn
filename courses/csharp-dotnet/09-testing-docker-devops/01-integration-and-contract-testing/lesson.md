# Module 9, Lesson 01 — Integration & Contract Testing

You've been writing tests since Module 2. This lesson names what kinds of tests you've actually been writing, fills in the one category you haven't (contract tests), and explains why real projects need more than one kind.

## The testing pyramid

```
        /\
       /  \      End-to-end (E2E) — few, slow, test the whole real system
      /----\
     /      \    Integration — some, medium speed, test several pieces together
    /--------\
   /          \  Unit — many, fast, test one thing in isolation
  /------------\
```

| Level | What it tests | Example from this course | Speed |
|---|---|---|---|
| **Unit** | One class/method, in isolation, dependencies faked/mocked | Module 2's `LibrarySystem` tests — a `Library` with a fake `ILateFeePolicy` | Milliseconds |
| **Integration** | Several real pieces working together | Module 5+'s `WebApplicationFactory` tests — real routing, real model binding, real (or in-memory) database | Tens–hundreds of milliseconds |
| **End-to-end** | The whole real system, as a user would experience it | A real browser driving a real deployed app against a real database | Seconds+ |

**The pyramid shape is deliberate**: many fast unit tests catch most bugs cheaply; fewer, slower integration tests catch the bugs that only show up when real pieces interact (wrong route, bad model binding, a missing DI registration); very few, expensive E2E tests catch what's left, sparingly, because they're slow and brittle.

You've been climbing this pyramid all course: Module 2's tests were **unit** tests (a class with fakes injected — `SpyNotifier`, `FixedAmountDiscount`). Everything from Module 5 onward using `WebApplicationFactory` is an **integration** test — it spins up your actual app, actual routing, and (Module 6 onward) an actual database, and sends it real HTTP requests.

## What makes a test "integration" rather than "unit"

```csharp
// UNIT — Library gets a FAKE dependency, nothing real touches disk/network
var library = new Library(new StandardLateFeePolicy());

// INTEGRATION — the REAL app, REAL routing, a REAL (in-memory) database
using var factory = new WebApplicationFactory<Program>();
var client = factory.CreateClient();
var response = await client.GetAsync("/api/students");
```

Neither is "better" — they test different things. A unit test tells you `Library.Borrow()`'s logic is correct. An integration test tells you the **whole pipeline** — routing, model binding, DI wiring, database queries — actually works together, which a unit test testing `Library` alone can never catch (e.g., forgetting to register a service, a typo in a route template).

## Contract testing — a third, narrower idea

A **contract test** checks that an API's *shape* — its response structure, required fields, status codes — matches what consumers expect, independent of the specific data values. This matters because **breaking an API's shape breaks every client calling it**, even if the underlying logic is "correct."

```csharp
[Fact]
public async Task GetStudent_ResponseHasRequiredContractFields()
{
    using var factory = new WebApplicationFactory<Program>();
    var client = factory.CreateClient();

    var response = await client.GetAsync("/api/students/1");
    response.EnsureSuccessStatusCode();

    using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    var root = doc.RootElement;

    // The CONTRACT: consumers depend on these fields existing, with these types —
    // not on any particular student's name or grade.
    Assert.True(root.TryGetProperty("id", out var id) && id.ValueKind == JsonValueKind.Number);
    Assert.True(root.TryGetProperty("name", out var name) && name.ValueKind == JsonValueKind.String);
    Assert.True(root.TryGetProperty("grade", out var grade) && grade.ValueKind == JsonValueKind.Number);
}
```

This is deliberately different from a normal integration test asserting `student.Name == "Ali"` — a contract test doesn't care *which* student came back, only that the **shape** is right: the right fields, with the right JSON types. If a future change accidentally renames `grade` to `score`, or changes it from a number to a string, this test fails immediately — catching a breaking change before it reaches whoever consumes this API (a frontend team, a mobile app, another service), often before those teams would otherwise discover it themselves.

## `System.Text.Json.JsonDocument` — inspecting JSON without a fixed type

Module 4's `JsonSerializer.Deserialize<T>` needs a known target type. `JsonDocument.Parse(...)` instead gives you a low-level, navigable view of *any* JSON — exactly what contract testing needs, since you're checking structure, not deserializing into a specific shape you're assuming is correct:

```csharp
using JsonDocument doc = JsonDocument.Parse(jsonString);
JsonElement root = doc.RootElement;
bool hasField = root.TryGetProperty("fieldName", out JsonElement value);
JsonValueKind kind = value.ValueKind;    // String, Number, True, False, Null, Object, Array
```

## Why this matters for real teams

When a frontend team and a backend team work on the same API independently, contract tests are the safety net that lets both move fast — the backend can refactor internals freely (Module 2's whole point), but a contract test immediately flags it if a refactor accidentally also changed what consumers actually depend on.

## Summary
- **Unit tests**: one class, fakes for dependencies, fast, many. **Integration tests**: real pieces working together (routing, DI, database), via `WebApplicationFactory`. **E2E tests**: the whole real system — few, slow, last resort.
- You've written unit tests since Module 2 and integration tests since Module 5, without necessarily naming them that way.
- **Contract tests** check an API response's *shape* (fields, types, status codes) rather than specific data values — they catch breaking changes to what consumers actually depend on.
- `JsonDocument.Parse(...)` + `TryGetProperty`/`ValueKind` inspects JSON structure without needing a fixed target type.
