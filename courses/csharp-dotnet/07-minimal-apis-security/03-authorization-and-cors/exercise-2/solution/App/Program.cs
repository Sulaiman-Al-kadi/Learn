using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
const string SigningKey = "exercise-signing-key-please-be-at-least-32-bytes-long";
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
        };
    });
builder.Services.AddAuthorization(options =>
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin")));

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/login", (LoginRequest body) =>
    Issue(new Claim(ClaimTypes.Name, body.Username), new Claim(ClaimTypes.Role, body.Username == "sara" ? "admin" : "user")));

app.MapGet("/reports", () => "reports").RequireAuthorization();
app.MapDelete("/reports", () => "deleted").RequireAuthorization("AdminOnly");

app.Run();

string Issue(params Claim[] claims)
{
    var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddMinutes(30),
        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
    return new JwtSecurityTokenHandler().WriteToken(token);
}

public record LoginRequest(string Username);

public partial class Program { }
