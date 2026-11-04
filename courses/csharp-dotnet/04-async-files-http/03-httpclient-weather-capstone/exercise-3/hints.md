## Hint 1
`_http.PostAsJsonAsync("/todos", new NewTodo(title, false))` serializes the body for you.

## Hint 2
Call `EnsureSuccessStatusCode()` before reading the response.
