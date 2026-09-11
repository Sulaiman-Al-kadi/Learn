// Capstone — Role-based authorization + CORS
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

// TODO: AddAuthorization with an "AdminOnly" policy requiring the "Admin" role

// TODO: AddCors with an "AllowFrontend" policy: WithOrigins("https://example.com"), AllowAnyHeader, AllowAnyMethod

var app = builder.Build();
// TODO: app.UseCors("AllowFrontend");  -- before UseAuthentication/UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

// TODO: POST /login (LoginRequest) -> match ali/secret123 -> "User", admin/adminpass -> "Admin",
//       else Results.Unauthorized(). Issue a JWT with Name AND Role claims.

// TODO: GET /tasks, .RequireAuthorization() -> new[] { "Buy milk", "Write report" }

// TODO: DELETE /tasks/{id:int}, .RequireAuthorization("AdminOnly") -> Results.NoContent()

app.Run();

public record LoginRequest(string Username, string Password);

public partial class Program { }
