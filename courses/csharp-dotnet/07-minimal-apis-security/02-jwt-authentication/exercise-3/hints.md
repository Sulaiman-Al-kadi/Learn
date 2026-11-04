## Hint 1
`users.TryGetValue(body.Username, out var stored) && stored == body.Password` covers both failure cases.

## Hint 2
`Results.Unauthorized()` sends the 401, and `Results.Ok(token)` sends the token.
