// Exercise 02 — JWT-protected TaskApi (solution)
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

app.MapPost("/login", (LoginRequest request) =>
{
    if (request.Username != "ali" || request.Password != "secret123")
    {
        return Results.Unauthorized();
    }

    var claims = new[] { new Claim(ClaimTypes.Name, request.Username) };
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddMinutes(30), signingCredentials: credentials);

    return Results.Ok(new JwtSecurityTokenHandler().WriteToken(token));
});

app.MapGet("/tasks", () => new[] { "Buy milk", "Write report" })
    .RequireAuthorization();

app.MapGet("/tasks/whoami", (ClaimsPrincipal user) => user.Identity!.Name)
    .RequireAuthorization();

app.Run();

public record LoginRequest(string Username, string Password);

public partial class Program { }
