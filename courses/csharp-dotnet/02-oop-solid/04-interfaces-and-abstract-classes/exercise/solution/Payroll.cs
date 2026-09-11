// Exercise 04 — IPayable interface (solution)

public interface IPayable
{
    double Amount { get; }
    string Describe();
}

public class Invoice : IPayable
{
    public string ClientName { get; set; } = "";
    public double Amount { get; set; }

    public string Describe() => $"Invoice for {ClientName}: {Amount}";
}

public class Employee : IPayable
{
    public string Name { get; set; } = "";
    public double Amount { get; set; }

    public string Describe() => $"{Name}'s pay: {Amount}";
}

public static class Payroll
{
    public static double Total(List<IPayable> payables)
    {
        double total = 0;
        foreach (IPayable p in payables)
        {
            total += p.Amount;
        }
        return total;
    }
}
