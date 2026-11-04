## Hint 1
`builder.Services.AddAuthorization(o => o.AddPolicy("AdminOnly", p => p.RequireRole("admin")));` defines it.

## Hint 2
`.RequireAuthorization("AdminOnly")` applies it. A logged-in user without the role gets 403, not 401.
