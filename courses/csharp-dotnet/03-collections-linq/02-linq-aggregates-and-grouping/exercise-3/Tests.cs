using Xunit;

public class SalesSummaryTests
{
    private static readonly List<Sale> Sales = [new("West", 40m), new("East", 30m), new("East", 25m), new("North", 5m)];

    [Fact] public void Totals_AddUp()
    {
        var totals = SalesSummary.TotalByRegion(Sales);
        Assert.Equal(55m, totals["East"]);
        Assert.Equal(3, totals.Count);
    }
    [Fact] public void BestRegion_UsesTotalNotSingleSale() => Assert.Equal("East", SalesSummary.BestRegion(Sales));
    [Fact] public void BestRegion_Empty() => Assert.Null(SalesSummary.BestRegion([]));
    [Theory] [InlineData(39, true)] [InlineData(40, false)]
    public void HasBigSale_IsStrict(int limit, bool expected) => Assert.Equal(expected, SalesSummary.HasBigSale(Sales, limit));
}
