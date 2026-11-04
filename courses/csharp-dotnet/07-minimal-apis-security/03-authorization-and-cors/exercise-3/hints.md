## Hint 1
`builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins("https://app.example.com")));` defines the rule.

## Hint 2
`app.UseCors();` switches it on, before the endpoints.
