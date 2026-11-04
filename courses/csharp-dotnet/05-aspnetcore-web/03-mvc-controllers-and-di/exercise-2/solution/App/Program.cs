var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<SessionTracker>();
builder.Services.AddScoped<RequestTracker>();

var app = builder.Build();

app.MapGet("/ids", (SessionTracker session, RequestTracker request) => new Ids(session.Id, request.Id));

app.Run();

public class Tracker { public Guid Id { get; } = Guid.NewGuid(); }
public class SessionTracker : Tracker { }
public class RequestTracker : Tracker { }
public record Ids(Guid Session, Guid Request);

public partial class Program { }
