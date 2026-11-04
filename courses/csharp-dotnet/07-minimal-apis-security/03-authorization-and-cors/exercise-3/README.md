# Exercise — CORS for one site

Let exactly one website call this API from a browser.

Allow the origin `https://app.example.com`, and no other. `GET /data` already exists.

Remember that CORS is enforced by browsers. The server only sends the headers, so the tests look at the response headers.
