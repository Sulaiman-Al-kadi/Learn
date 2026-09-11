using Xunit;

public class MoneyTests
{
    [Fact]
    public void RecordEquality_SameValues_AreEqual()
    {
        Assert.Equal(new Money(10, "USD"), new Money(10, "USD"));
    }

    [Fact]
    public void RecordEquality_DifferentAmount_NotEqual()
    {
        Assert.NotEqual(new Money(10, "USD"), new Money(20, "USD"));
    }

    [Fact]
    public void Add_SameCurrency_Sums()
    {
        var a = new Money(10, "USD");
        var b = new Money(5, "USD");
        Assert.Equal(new Money(15, "USD"), a.Add(b));
    }

    [Fact]
    public void Add_DoesNotMutateOriginal()
    {
        var a = new Money(10, "USD");
        a.Add(new Money(5, "USD"));
        Assert.Equal(new Money(10, "USD"), a);
    }

    [Fact]
    public void Add_DifferentCurrency_Throws()
    {
        var a = new Money(10, "USD");
        var b = new Money(5, "EUR");
        Assert.Throws<ArgumentException>(() => a.Add(b));
    }

    [Fact]
    public void ApplyDiscount_ReducesAmount()
    {
        var m = new Money(100, "USD");
        Assert.Equal(new Money(90, "USD"), m.ApplyDiscount(10));
    }

    [Fact]
    public void ApplyDiscount_DoesNotMutateOriginal()
    {
        var m = new Money(100, "USD");
        m.ApplyDiscount(50);
        Assert.Equal(new Money(100, "USD"), m);
    }
}
