# Exercise — Update and delete notes

Finish the API from the previous exercise.

1. `PUT /notes/{id:int}`: replace the text and respond `204`, or `404`.
2. `DELETE /notes/{id:int}`: remove the note and respond `204`, or `404`.
3. `GET /notes?page=1&size=2`: one page, oldest id first. `size` defaults to 10.

One note is seeded at startup.
