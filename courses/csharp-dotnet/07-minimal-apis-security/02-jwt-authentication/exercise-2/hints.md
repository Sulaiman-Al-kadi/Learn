## Hint 1
`Issue(new Claim(ClaimTypes.Name, body.Username))` makes the token.

## Hint 2
Inject `ClaimsPrincipal user` into the handler. `user.Identity!.Name` is the name claim.

## Hint 3
Finish the endpoint with `.RequireAuthorization()`.
