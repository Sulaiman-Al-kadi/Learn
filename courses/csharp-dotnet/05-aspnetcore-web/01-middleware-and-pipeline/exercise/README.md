# Exercise — Custom middleware + endpoints

Write the whole app in `App/Program.cs` (top-level statements, like every `Program.cs` this course).

## Middleware
A single custom middleware, registered **before** any endpoints, that adds a response header to every request:
```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Powered-By", "LearnLab");
    await next();
});
```
(You can copy this — the point of the exercise is the endpoints and getting the pipeline order right, not memorizing header syntax.)

## Endpoints
| Route | Method | Returns |
|---|---|---|
| `/hello` | GET | the string `"Hello, World!"` |
| `/students` | GET | a `string[]` (or `List<string>`) containing exactly `"Ali"`, `"Sara"`, `"Omar"`, in that order |
| `/echo/{text}` | GET | the `text` route parameter, returned as-is |

## Rules
- The middleware must be registered with `app.Use(...)` **before** the `app.MapGet(...)` calls — middleware order matters (lesson content).
- `/echo/{text}` — the handler takes a `string text` parameter; ASP.NET Core binds it from the `{text}` route segment automatically because the parameter name matches the route placeholder name.
- A request to an unmapped route (e.g. `/nope`) should naturally 404 — you don't need to write anything special for that; it's the framework's default behavior when no endpoint matches.

## How it's tested
`Tests.cs` uses `WebApplicationFactory<Program>` to run your whole app in memory and send it real requests — see the lesson for why this works without a real server or network.
