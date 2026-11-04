public record Product(string Name, decimal Price, bool InStock);

public static class ProductFilters
{
    public static List<string> InStockNames(List<Product> products)
    {
        throw new NotImplementedException();
    }

    public static List<decimal> PricesWithTax(List<Product> products, decimal rate)
    {
        throw new NotImplementedException();
    }

    public static List<Product> ByPriceThenName(List<Product> products)
    {
        throw new NotImplementedException();
    }
}
