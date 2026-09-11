# Module 7, Lesson 02 — JWT Authentication

Every endpoint you've built so far is open to anyone. Real APIs need to know **who** is calling, and whether **they're allowed** to do what they're asking. This lesson covers the standard modern approach: **JWT (JSON Web Token)** authentication.

## Authentication vs. authorization — two different questions

- **Authentication**: *Who are you?* (proving identity — usually via a login step)
- **Authorization**: *Are you allowed to do this?* (a decision made once identity is known)

This lesson focuses on authentication; lesson 03 covers authorization policies in more depth.

## What a JWT actually is

A JWT is a compact, signed piece of text representing a set of **claims** (facts about the user — a username, an id, a role) that a server issued and cryptographically signed. It has three dot-separated parts: `header.payload.signature`. Anyone can **read** the payload (it's just base64, not encrypted) — but only someone holding the **signing key** can produce a signature that the server will accept as genuine. That's the whole security model: the token can't be forged or tampered with undetected, even though its contents aren't secret.

You've already used one across Module 4's `HttpClient` calls — the `Authorization: Bearer <token>` header is how a JWT travels with a request.

## Issuing a token — the login endpoint

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

app.MapPost("/login", (LoginRequest request) =>
{
    // In a real app: look up the user, verify their password hash (Module 8 territory).
    if (request.Username != "ali" || request.Password != "secret123")
    {
        return Results.Unauthorized();
    }

    var claims = new[] { new Claim(ClaimTypes.Name, request.Username) };
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(30),
        signingCredentials: credentials);

    return Results.Ok(new JwtSecurityTokenHandler().WriteToken(token));
});

public record LoginRequest(string Username, string Password);
```

The client sends credentials once; the server verifies them and hands back a signed token. From then on, the client attaches that token to every subsequent request instead of re-sending a password.

## Validating tokens — protecting endpoints

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,                                                // reject expired tokens
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),   // must match the signing key used to issue tokens
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();       // WHO is this? (reads and validates the token, if present)
app.UseAuthorization();         // ARE they allowed? (checked against [Authorize]/.RequireAuthorization())
```

Order matters here (lesson 01's middleware pipeline!) — authentication must run before authorization, since you can't decide "are they allowed" before knowing "who are they."

## Requiring authentication on an endpoint

```csharp
app.MapGet("/profile", (ClaimsPrincipal user) => $"Hello, {user.Identity!.Name}")
    .RequireAuthorization();

// or, on an MVC controller (Module 5, lesson 03):
[Authorize]
[HttpGet]
public IActionResult GetProfile() => Ok($"Hello, {User.Identity!.Name}");
```

`.RequireAuthorization()` (Minimal APIs) / `[Authorize]` (MVC controllers) means: "reject this request with `401 Unauthorized` unless a valid token was presented." A `ClaimsPrincipal` parameter (or the controller's `User` property) gives you access to the authenticated user's claims — the same username you put into the token at login.

## What the client does

```csharp
var response = await client.PostAsJsonAsync("/login", new LoginRequest("ali", "secret123"));
string token = await response.Content.ReadAsStringAsync();

client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Trim('"'));

var profile = await client.GetAsync("/profile");     // now succeeds — the token travels with every request from here on
```

This is Module 4's `HttpClient` again, just with one more header set once, reused for every subsequent call.

## The signing key — a real secret

Unlike the token's claims (readable by anyone), the **signing key** must stay secret — anyone who has it can forge valid tokens. Never hardcode it in source control for a real app; it belongs in configuration (Module 8 covers configuration properly) or a secrets manager. For this course's exercises, a hardcoded test key is fine — you're learning the mechanism, not shipping it.

## Summary
- **Authentication** = who you are; **authorization** = what you're allowed to do.
- A JWT is a signed, tamper-evident (but not secret/encrypted) set of claims, sent as `Authorization: Bearer <token>`.
- A `/login` endpoint verifies credentials and issues a signed token.
- `AddAuthentication().AddJwtBearer(...)` configures how incoming tokens are validated; `UseAuthentication()` then `UseAuthorization()` in the pipeline (order matters).
- `.RequireAuthorization()` / `[Authorize]` reject unauthenticated requests with `401`.
- The signing key is a genuine secret — never hardcode it in a real app.
