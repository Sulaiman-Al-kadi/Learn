// Exercise 04 — IPayable interface
// See README.md.

public interface IPayable
{
    double Amount { get; }
    string Describe();
}

public class Invoice : IPayable
{
    // TODO: ClientName, Amount properties; Describe()
    public string ClientName { get; set; } = "";
    public double Amount { get; set; }
    public string Describe() => throw new NotImplementedException();
}

public class Employee : IPayable
{
    // TODO: Name, Amount properties; Describe()
    public string Name { get; set; } = "";
    public double Amount { get; set; }
    public string Describe() => throw new NotImplementedException();
}

public static class Payroll
{
    // TODO: static double Total(List<IPayable> payables) — sum Amount with a loop
    public static double Total(List<IPayable> payables)
    {
        throw new NotImplementedException();
    }
}
