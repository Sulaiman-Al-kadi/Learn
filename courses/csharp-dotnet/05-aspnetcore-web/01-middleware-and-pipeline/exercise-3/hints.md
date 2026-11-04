## Hint 1
`context.Request.Path.StartsWithSegments("/admin")` tests the path.

## Hint 2
To stop the pipeline, set `context.Response.StatusCode = 401` and `return` without calling `next`.
