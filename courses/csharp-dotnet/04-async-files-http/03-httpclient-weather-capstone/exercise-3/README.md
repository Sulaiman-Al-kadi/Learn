# Exercise — Create a todo

Send data to an API.

`CreateAsync(title)` sends `POST /todos` with the JSON body `{ "title": ... , "done": false }`, and returns the `Todo` from the response, which includes the new id. Throw on any failure status.
