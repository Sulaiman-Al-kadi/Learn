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
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

var users = new Dictionary<string, string> { ["sara"] = "pass123" };

app.MapPost("/login", (LoginRequest body) =>
    users.TryGetValue(body.Username, out var stored) && stored == body.Password
        ? Results.Ok(Issue(new Claim(ClaimTypes.Name, body.Username)))
        : Results.Unauthorized());

app.MapGet("/secret", () => "the secret").RequireAuthorization();

app.Run();

string Issue(params Claim[] claims)
{
    var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddMinutes(30),
        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
    return new JwtSecurityTokenHandler().WriteToken(token);
}

public record LoginRequest(string Username, string Password);

public partial class Program { }
