# Exercise — Who am I?

JWT validation is set up, and `Issue(...)` signs a token from the claims you give it.

1. `POST /login`: body `{ "username": "..." }`. Respond with a token holding a `ClaimTypes.Name` claim.
2. `GET /me`: requires a valid token, and responds with the username from the token as plain text.
