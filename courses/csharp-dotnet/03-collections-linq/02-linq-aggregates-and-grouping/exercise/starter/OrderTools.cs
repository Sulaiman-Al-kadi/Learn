// Exercise 02 — Order analytics toolkit
// See README.md. LINQ only, no manual foreach.

public record Order(string Customer, string Product, double Amount);

public static class OrderTools
{
    public static double TotalRevenue(List<Order> orders)
    {
        throw new NotImplementedException();
    }

    public static double AverageOrderAmount(List<Order> orders)
    {
        throw new NotImplementedException();
    }

    public static int CountBigOrders(List<Order> orders, double threshold)
    {
        throw new NotImplementedException();
    }

    public static bool AnyOrderFrom(List<Order> orders, string customer)
    {
        throw new NotImplementedException();
    }

    public static Order? FindFirstOrderFrom(List<Order> orders, string customer)
    {
        throw new NotImplementedException();
    }

    public static List<string> CustomersSortedByTotalSpent(List<Order> orders)
    {
        throw new NotImplementedException();
    }
}
