public record Sale(string Region, decimal Amount);

public static class SalesSummary
{
    public static Dictionary<string, decimal> TotalByRegion(List<Sale> sales)
    {
        throw new NotImplementedException();
    }

    public static string? BestRegion(List<Sale> sales)
    {
        throw new NotImplementedException();
    }

    public static bool HasBigSale(List<Sale> sales, decimal limit)
    {
        throw new NotImplementedException();
    }
}
