# Capstone — Role-based authorization + CORS

Write the whole app in `App/Program.cs`. This extends lesson 02's `TaskApi` — reuse that pattern for the setup and `/login`/`/tasks` parts.

## Setup
Same signing key as lesson 02: `"exercise-signing-key-please-be-at-least-32-bytes-long"`.

Add a named policy and a CORS policy alongside the authentication setup:
```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("https://example.com").AllowAnyHeader().AllowAnyMethod());
});
```
Register CORS **before** authentication in the pipeline:
```csharp
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
```

## Users (hardcoded — no database yet)
| Username | Password | Role |
|---|---|---|
| `ali` | `secret123` | `User` |
| `admin` | `adminpass` | `Admin` |

## Endpoints

| Route | Method | Behavior |
|---|---|---|
| `/login` | POST | body `LoginRequest(string Username, string Password)`. Match against the table above; issue a JWT with **both** a `ClaimTypes.Name` claim (the username) and a `ClaimTypes.Role` claim (their role). Wrong credentials → `Results.Unauthorized()`. |
| `/tasks` | GET | `.RequireAuthorization()` (any authenticated user, no specific role) → `new[] { "Buy milk", "Write report" }` |
| `/tasks/{id:int}` | DELETE | `.RequireAuthorization("AdminOnly")` → `Results.NoContent()` |

## Rules
- A `User`-role token calling `DELETE /tasks/{id}` must get `403 Forbidden` (valid token, wrong role) — not `401`.
- An `Admin`-role token can call both `/tasks` (GET) and `/tasks/{id}` (DELETE).
- No token at all on either endpoint → `401 Unauthorized`.
