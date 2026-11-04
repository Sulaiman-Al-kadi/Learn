public record Sale(string Region, decimal Amount);

public static class SalesSummary
{
    public static Dictionary<string, decimal> TotalByRegion(List<Sale> sales)
    {
        return sales.GroupBy(s => s.Region).ToDictionary(g => g.Key, g => g.Sum(s => s.Amount));
    }

    public static string? BestRegion(List<Sale> sales)
    {
        return TotalByRegion(sales).OrderByDescending(kv => kv.Value).Select(kv => kv.Key).FirstOrDefault();
    }

    public static bool HasBigSale(List<Sale> sales, decimal limit)
    {
        return sales.Any(s => s.Amount > limit);
    }
}
