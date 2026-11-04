public record Product(string Name, decimal Price, bool InStock);

public static class ProductFilters
{
    public static List<string> InStockNames(List<Product> products)
    {
        return products.Where(p => p.InStock).OrderBy(p => p.Name).Select(p => p.Name).ToList();
    }

    public static List<decimal> PricesWithTax(List<Product> products, decimal rate)
    {
        return products.Select(p => p.Price * (1 + rate)).ToList();
    }

    public static List<Product> ByPriceThenName(List<Product> products)
    {
        return products.OrderBy(p => p.Price).ThenBy(p => p.Name).ToList();
    }
}
