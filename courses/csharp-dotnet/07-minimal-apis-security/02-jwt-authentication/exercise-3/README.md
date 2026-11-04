# Exercise — Check the password

Only hand out tokens to people who prove who they are.

`POST /login` takes `{ "username": ..., "password": ... }` and checks it against the `users` dictionary.

- wrong user or password: `401`
- correct: `200` with a token, using `Issue(...)`

`GET /secret` requires a token and returns `the secret`.
