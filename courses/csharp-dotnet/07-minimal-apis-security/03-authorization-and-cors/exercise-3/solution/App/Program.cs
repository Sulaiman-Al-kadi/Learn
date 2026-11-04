var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy.WithOrigins("https://app.example.com")));

var app = builder.Build();
app.UseCors();

app.MapGet("/data", () => "data");

app.Run();

public partial class Program { }
