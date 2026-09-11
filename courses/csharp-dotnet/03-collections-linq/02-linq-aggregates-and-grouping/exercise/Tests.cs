using Xunit;

public class OrderToolsTests
{
    private static List<Order> SampleOrders() => new List<Order>
    {
        new Order("Sara", "Book", 20),
        new Order("Ali", "Pen", 5),
        new Order("Sara", "Pen", 5),
        new Order("Omar", "Book", 20),
        new Order("Ali", "Book", 20),
        new Order("Sara", "Laptop", 800),
    };

    [Fact]
    public void TotalRevenue_SumsAllAmounts()
    {
        Assert.Equal(870, OrderTools.TotalRevenue(SampleOrders()));
    }

    [Fact]
    public void AverageOrderAmount_ComputesCorrectly()
    {
        Assert.Equal(145, OrderTools.AverageOrderAmount(SampleOrders()));
    }

    [Fact]
    public void CountBigOrders_CountsAboveThreshold()
    {
        Assert.Equal(4, OrderTools.CountBigOrders(SampleOrders(), 15));
    }

    [Fact]
    public void AnyOrderFrom_True_WhenCustomerExists()
    {
        Assert.True(OrderTools.AnyOrderFrom(SampleOrders(), "Omar"));
    }

    [Fact]
    public void AnyOrderFrom_False_WhenCustomerDoesNotExist()
    {
        Assert.False(OrderTools.AnyOrderFrom(SampleOrders(), "Zaid"));
    }

    [Fact]
    public void FindFirstOrderFrom_ReturnsFirstMatchInOriginalOrder()
    {
        var order = OrderTools.FindFirstOrderFrom(SampleOrders(), "Ali");
        Assert.NotNull(order);
        Assert.Equal("Pen", order!.Product);
    }

    [Fact]
    public void FindFirstOrderFrom_ReturnsNull_WhenNoMatch()
    {
        Assert.Null(OrderTools.FindFirstOrderFrom(SampleOrders(), "Zaid"));
    }

    [Fact]
    public void CustomersSortedByTotalSpent_OrdersByTotalDescending()
    {
        var result = OrderTools.CustomersSortedByTotalSpent(SampleOrders());
        Assert.Equal(new List<string> { "Sara", "Ali", "Omar" }, result);
    }
}
