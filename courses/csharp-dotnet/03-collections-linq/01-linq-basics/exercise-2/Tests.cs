using Xunit;

public class ProductFilterTests
{
    private static readonly List<Product> Items =
    [
        new("Tea", 10m, true), new("Coffee", 20m, false), new("Apple", 10m, true), new("Bread", 5m, true),
    ];

    [Fact] public void InStockNames_FiltersAndSorts() => Assert.Equal(["Apple", "Bread", "Tea"], ProductFilters.InStockNames(Items));
    [Fact] public void InStockNames_Empty() => Assert.Empty(ProductFilters.InStockNames([]));
    [Fact] public void PricesWithTax_KeepsOrder() => Assert.Equal([11.0m, 22.0m, 11.0m, 5.5m], ProductFilters.PricesWithTax(Items, 0.1m));
    [Fact] public void ByPriceThenName_BreaksTies() => Assert.Equal(["Bread", "Apple", "Tea", "Coffee"], ProductFilters.ByPriceThenName(Items).Select(p => p.Name));
}
