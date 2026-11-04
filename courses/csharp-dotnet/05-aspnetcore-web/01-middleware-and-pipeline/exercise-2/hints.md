## Hint 1
`app.Use(async (context, next) => { ...; await next(context); })` wraps every request.

## Hint 2
Set `context.Response.Headers["X-Request-Count"]` **before** `await next(context)`: once the body starts, headers can no longer change.
