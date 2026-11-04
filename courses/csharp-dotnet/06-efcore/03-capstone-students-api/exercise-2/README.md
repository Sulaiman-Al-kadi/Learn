# Exercise — Notes API on SQLite

Join Minimal APIs and EF Core. The context is registered for you, on an in-memory SQLite connection that lives as long as the app.

1. `POST /notes`: save a note from the JSON body and respond `201` with it.
2. `GET /notes`: all notes, newest id first.
3. `GET /notes/{id:int}`: the note, or `404`.

A `DbContext` parameter on an endpoint is injected per request.
