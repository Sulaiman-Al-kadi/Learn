// Exercise 02 — JWT-protected TaskApi
// See README.md.
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

// TODO: POST /login (LoginRequest) -> issue a JWT if credentials match "ali"/"secret123",
//       else Results.Unauthorized()

// TODO: GET /tasks, .RequireAuthorization() -> new[] { "Buy milk", "Write report" }

// TODO: GET /tasks/whoami, .RequireAuthorization() -> (ClaimsPrincipal user) => user.Identity!.Name

app.Run();

public record LoginRequest(string Username, string Password);

public partial class Program { }
