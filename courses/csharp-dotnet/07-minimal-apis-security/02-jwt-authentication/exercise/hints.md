## Hint 1
Login checks credentials first, then builds and signs the token:
```csharp
app.MapPost("/login", (LoginRequest request) =>
{
    if (request.Username != "ali" || request.Password != "secret123") return Results.Unauthorized();

    var claims = new[] { new Claim(ClaimTypes.Name, request.Username) };
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddMinutes(30), signingCredentials: credentials);

    return Results.Ok(new JwtSecurityTokenHandler().WriteToken(token));
});
```

## Hint 2
```csharp
app.MapGet("/tasks", () => new[] { "Buy milk", "Write report" })
    .RequireAuthorization();
```

## Hint 3
```csharp
app.MapGet("/tasks/whoami", (ClaimsPrincipal user) => user.Identity!.Name)
    .RequireAuthorization();
```
`ClaimsPrincipal` as a handler parameter is automatically bound to the current authenticated user — no attribute needed, similar to how `HttpContext` can be injected.
