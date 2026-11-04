using Microsoft.Extensions.Logging;

public class Payments
{
    private readonly ILogger<Payments> _logger;

    public Payments(ILogger<Payments> logger) => _logger = logger;

    public bool Charge(string user, decimal amount)
    {
        throw new NotImplementedException();
    }
}
