# Exercise — Product filters

Practice `Where`, `Select`, `OrderBy`, and `ThenBy` on a small catalog. Each method should be one LINQ chain ending in `.ToList()`.

1. `InStockNames`: names of products in stock, A to Z.
2. `PricesWithTax`: every price multiplied by `1 + rate`, in the original order.
3. `ByPriceThenName`: products from cheapest to most expensive. Equal prices go A to Z by name.
