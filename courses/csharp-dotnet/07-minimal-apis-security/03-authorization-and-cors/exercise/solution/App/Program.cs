// Capstone — Role-based authorization + CORS (solution)
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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("https://example.com").AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/login", (LoginRequest request) =>
{
    string? role = (request.Username, request.Password) switch
    {
        ("ali", "secret123") => "User",
        ("admin", "adminpass") => "Admin",
        _ => null,
    };
    if (role is null) return Results.Unauthorized();

    var claims = new[]
    {
        new Claim(ClaimTypes.Name, request.Username),
        new Claim(ClaimTypes.Role, role),
    };
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddMinutes(30), signingCredentials: credentials);

    return Results.Ok(new JwtSecurityTokenHandler().WriteToken(token));
});

app.MapGet("/tasks", () => new[] { "Buy milk", "Write report" })
    .RequireAuthorization();

app.MapDelete("/tasks/{id:int}", (int id) => Results.NoContent())
    .RequireAuthorization("AdminOnly");

app.Run();

public record LoginRequest(string Username, string Password);

public partial class Program { }
