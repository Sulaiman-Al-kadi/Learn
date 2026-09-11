// Exercise 02 — Order analytics toolkit (solution)

public record Order(string Customer, string Product, double Amount);

public static class OrderTools
{
    public static double TotalRevenue(List<Order> orders)
    {
        return orders.Sum(o => o.Amount);
    }

    public static double AverageOrderAmount(List<Order> orders)
    {
        return orders.Average(o => o.Amount);
    }

    public static int CountBigOrders(List<Order> orders, double threshold)
    {
        return orders.Count(o => o.Amount > threshold);
    }

    public static bool AnyOrderFrom(List<Order> orders, string customer)
    {
        return orders.Any(o => o.Customer == customer);
    }

    public static Order? FindFirstOrderFrom(List<Order> orders, string customer)
    {
        return orders.FirstOrDefault(o => o.Customer == customer);
    }

    public static List<string> CustomersSortedByTotalSpent(List<Order> orders)
    {
        return orders
            .GroupBy(o => o.Customer)
            .OrderByDescending(g => g.Sum(o => o.Amount))
            .Select(g => g.Key)
            .ToList();
    }
}
