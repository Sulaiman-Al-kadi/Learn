# Exercise — Quote client

Call a small API. The tests swap in a fake `HttpMessageHandler`, so no real network is used.

`GetAsync(id)` sends `GET /quotes/{id}` and returns:

- the `Quote` on success
- `null` on 404
- an exception for any other failure

The tests set `BaseAddress`, so use a relative path.
