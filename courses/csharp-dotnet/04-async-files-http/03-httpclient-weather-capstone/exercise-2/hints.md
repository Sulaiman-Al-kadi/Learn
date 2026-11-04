## Hint 1
Use `GetAsync`, not `GetFromJsonAsync`, so you can look at the status code first.

## Hint 2
`response.StatusCode == HttpStatusCode.NotFound` is the `null` case. `EnsureSuccessStatusCode()` handles the rest.

## Hint 3
`await response.Content.ReadFromJsonAsync<Quote>()` reads the body.
