# Exercise — JWT-protected TaskApi

Write the whole app in `App/Program.cs`.

## Setup
Use this exact signing key (a constant, top of the file, so the tests can be deterministic): `"exercise-signing-key-please-be-at-least-32-bytes-long"`.

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
const string SigningKey = "exercise-signing-key-please-be-at-least-32-bytes-long";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey)),
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
```
(You can copy this setup block — the exercise is in the endpoints below.)

## Endpoints

| Route | Method | Behavior |
|---|---|---|
| `/login` | POST | body: `LoginRequest(string Username, string Password)`. If `Username == "ali"` and `Password == "secret123"`, issue a JWT (claim: `ClaimTypes.Name` = username, expires in 30 minutes) and return it with `Results.Ok(token)`. Otherwise `Results.Unauthorized()`. |
| `/tasks` | GET | `.RequireAuthorization()` — returns `new[] { "Buy milk", "Write report" }`. Requires a valid token. |
| `/tasks/whoami` | GET | `.RequireAuthorization()` — returns the caller's username via a `ClaimsPrincipal user` parameter: `user.Identity!.Name`. |

## Rules
- `record LoginRequest(string Username, string Password);` — put it after `app.Run();`, same as every other file this course.
- Use `JwtSecurityTokenHandler().WriteToken(...)` to produce the token string, exactly as in the lesson.
- Both `/tasks` endpoints must be unreachable (401) without a valid `Authorization: Bearer <token>` header.
