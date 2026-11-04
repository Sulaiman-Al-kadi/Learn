# Exercise — Calculator endpoints

Practice status codes and binding.

1. `GET /add/{a:int}/{b:int}`: returns the sum as a number.
2. `GET /divide?a=..&b=..`: returns `a / b`. When `b` is 0, respond `400` with the text `Cannot divide by zero`.
3. `POST /sum`: the body is a JSON array of numbers. Respond with their sum.
