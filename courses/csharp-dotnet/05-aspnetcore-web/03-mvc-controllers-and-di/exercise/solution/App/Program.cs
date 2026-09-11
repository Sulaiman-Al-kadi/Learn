// Program.cs is fixed infrastructure — wiring up DI and MVC controllers.
// You write the actual interface, repository, and controller in StudentsApi.cs.

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IStudentRepository, InMemoryStudentRepository>();
builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();
app.Run();

public partial class Program { }
