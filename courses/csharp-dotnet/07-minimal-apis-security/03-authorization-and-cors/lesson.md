# Module 7 Capstone — Authorization Policies & CORS

Two last pieces complete the security picture: deciding **what** an authenticated user can do (beyond just "are they logged in"), and controlling **who else's web pages** are allowed to call your API from a browser.

## Role-based authorization

Add a role claim when issuing the token:

```csharp
var claims = new[]
{
    new Claim(ClaimTypes.Name, username),
    new Claim(ClaimTypes.Role, "Admin"),          // or "User", "Manager", whatever your app needs
};
```

Then require it on an endpoint:

```csharp
app.MapDelete("/students/{id:int}", (int id) => Results.NoContent())
    .RequireAuthorization(policy => policy.RequireRole("Admin"));

// or, on an MVC controller:
[Authorize(Roles = "Admin")]
[HttpDelete("{id:int}")]
public IActionResult Delete(int id) => NoContent();
```

A request with a **valid token but the wrong role** gets `403 Forbidden` — different from `401 Unauthorized` (Module 7, lesson 02). This distinction matters: `401` means "we don't know who you are" (no token, or an invalid/expired one); `403` means "we know exactly who you are, and you're not allowed to do this."

## Named policies — reusable authorization rules

Repeating `policy.RequireRole("Admin")` on every admin endpoint gets tedious. Define it once:

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});
```

```csharp
app.MapDelete("/students/{id:int}", (int id) => Results.NoContent())
    .RequireAuthorization("AdminOnly");
```

Policies can express more than just roles — "must be over 18," "must own this specific resource," custom logic entirely — but role-based policies cover most real needs, including everything this course asks of you.

## CORS — Cross-Origin Resource Sharing

By default, a browser **blocks** JavaScript running on one website from calling an API hosted on a different origin (different domain, port, or protocol) — this is a core browser security feature, not something your API does. **CORS** is how your API explicitly grants specific origins permission to call it anyway.

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("https://myfrontend.com")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();
app.UseCors("AllowFrontend");           // must be registered before endpoints are reached (lesson 01's pipeline order)
```

| Piece | Meaning |
|---|---|
| `WithOrigins(...)` | Which website(s) are allowed to call this API from browser JavaScript. |
| `AllowAnyHeader()` / `AllowAnyMethod()` | Permit any request headers / any HTTP methods from those origins. |
| `app.UseCors(...)` | Registers the CORS middleware in the pipeline — it must run early enough to handle the browser's request. |

**This only affects browsers.** `HttpClient` (Module 4), Postman, `curl`, or your own test code are never blocked by CORS — it's a rule browsers enforce on behalf of the pages they render, not a server-side access restriction in general. Skipping CORS setup won't stop a determined attacker from calling your API directly; it stops a *browser* from letting some *other* website's JavaScript quietly call your API using a logged-in user's credentials without permission.

## Why order matters, once more

```csharp
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

Same lesson 01 principle: middleware order is the pipeline's actual behavior, not a suggestion. CORS typically comes early (before auth), so that even a rejected cross-origin preflight request gets the right CORS headers back.

## Summary
- Add a role claim at token-issuance time; require it with `.RequireAuthorization(policy => policy.RequireRole("Admin"))` or `[Authorize(Roles = "Admin")]`.
- Wrong role on a valid token → `403 Forbidden`. No/invalid token → `401 Unauthorized`. Different meanings, don't confuse them.
- Named policies (`AddPolicy("AdminOnly", ...)`) avoid repeating the same rule on every endpoint.
- **CORS** controls which browser-based origins may call your API from JavaScript — it's a browser-enforced rule, not a general server-side security boundary (non-browser clients are unaffected).
- `app.UseCors(...)` must be registered in the right place in the pipeline, same as every other middleware.

## The capstone

Open the exercise. You'll extend Module 7 lesson 02's `TaskApi` with an admin-only endpoint (role-based policy) and a CORS policy — the final, most complete version of the API this module has been building toward, matching the university plan's "REST service secured by JWT."
