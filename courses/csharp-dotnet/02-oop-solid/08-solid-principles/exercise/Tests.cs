using Xunit;

// A test double — only possible because OrderProcessor depends on the INotifier
// interface rather than a concrete class. This IS the point of DIP.
public class SpyNotifier : INotifier
{
    public List<string> Sent = new List<string>();
    public void Send(string message) => Sent.Add(message);
}

// A brand-new strategy defined entirely in the test file. Checkout.CalculateTotal
// needs zero changes to support it — this IS the point of OCP.
public class FixedAmountDiscount : IDiscountStrategy
{
    private readonly double _amount;
    public FixedAmountDiscount(double amount) => _amount = amount;
    public double Apply(double subtotal) => subtotal - _amount;
}

public class SolidTests
{
    [Fact]
    public void OrderProcessor_SendsThroughInjectedNotifier()
    {
        var spy = new SpyNotifier();
        var processor = new OrderProcessor(spy);

        processor.Complete("ORD-1");

        Assert.Single(spy.Sent);
        Assert.Equal("Order ORD-1 completed", spy.Sent[0]);
    }

    [Fact]
    public void OrderProcessor_DifferentNotifierInstances_AreIndependent()
    {
        var spyA = new SpyNotifier();
        var spyB = new SpyNotifier();
        new OrderProcessor(spyA).Complete("A");

        Assert.Single(spyA.Sent);
        Assert.Empty(spyB.Sent);
    }

    [Fact]
    public void NoDiscount_LeavesSubtotalUnchanged()
    {
        Assert.Equal(100, new NoDiscount().Apply(100));
    }

    [Fact]
    public void PercentageDiscount_ReducesCorrectly()
    {
        Assert.Equal(90, new PercentageDiscount(10).Apply(100));
    }

    [Fact]
    public void Checkout_WorksWithNoDiscount()
    {
        Assert.Equal(50, Checkout.CalculateTotal(50, new NoDiscount()));
    }

    [Fact]
    public void Checkout_WorksWithPercentageDiscount()
    {
        Assert.Equal(80, Checkout.CalculateTotal(100, new PercentageDiscount(20)));
    }

    [Fact]
    public void Checkout_WorksWithACompletelyNewStrategy_NoChangesNeeded()
    {
        // FixedAmountDiscount didn't exist when Checkout was written — that's OCP.
        Assert.Equal(70, Checkout.CalculateTotal(100, new FixedAmountDiscount(30)));
    }
}
