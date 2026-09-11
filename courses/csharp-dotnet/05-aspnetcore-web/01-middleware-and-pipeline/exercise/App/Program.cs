// Exercise 01 — Custom middleware + endpoints
// See README.md.

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// TODO: middleware that adds header "X-Powered-By": "LearnLab" to every response, then calls next()

// TODO: GET /hello -> "Hello, World!"

// TODO: GET /students -> new[] { "Ali", "Sara", "Omar" }

// TODO: GET /echo/{text} -> text  (route parameter, bound by matching name)

app.Run();

public partial class Program { }
