using Microsoft.Extensions.Logging;

public class Payments
{
    private readonly ILogger<Payments> _logger;

    public Payments(ILogger<Payments> logger) => _logger = logger;

    public bool Charge(string user, decimal amount)
    {
        if (amount <= 0)
        {
            _logger.LogWarning("Rejected charge for {User}", user);
            return false;
        }
        _logger.LogInformation("Charged {User} {Amount}", user, amount);
        return true;
    }
}
