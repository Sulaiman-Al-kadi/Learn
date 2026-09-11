## Hint 1
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

## Hint 2
`app.UseCors("AllowFrontend");` goes right after `var app = builder.Build();`, before `app.UseAuthentication();`.

## Hint 3
A C# tuple pattern switch is a clean way to map credentials to a role:
```csharp
string? role = (request.Username, request.Password) switch
{
    ("ali", "secret123") => "User",
    ("admin", "adminpass") => "Admin",
    _ => null,
};
if (role is null) return Results.Unauthorized();
```
Then include `new Claim(ClaimTypes.Role, role)` alongside the `Name` claim when building the token — same pattern as lesson 02, one more claim in the array.

## Hint 4
```csharp
app.MapGet("/tasks", () => new[] { "Buy milk", "Write report" })
    .RequireAuthorization();

app.MapDelete("/tasks/{id:int}", (int id) => Results.NoContent())
    .RequireAuthorization("AdminOnly");
```
