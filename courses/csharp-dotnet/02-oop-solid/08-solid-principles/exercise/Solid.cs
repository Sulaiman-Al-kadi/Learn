// Exercise 08 — DIP notifier + OCP discount strategy
// See README.md.

public interface INotifier
{
    void Send(string message);
}

public class OrderProcessor
{
    // TODO: private readonly INotifier field, set via the constructor
    // TODO: constructor OrderProcessor(INotifier notifier)
    public OrderProcessor(INotifier notifier)
    {
        throw new NotImplementedException();
    }

    // TODO: void Complete(string orderId) => calls notifier.Send($"Order {orderId} completed")
    public void Complete(string orderId)
    {
        throw new NotImplementedException();
    }
}

public interface IDiscountStrategy
{
    double Apply(double subtotal);
}

public class NoDiscount : IDiscountStrategy
{
    public double Apply(double subtotal)
    {
        throw new NotImplementedException();
    }
}

public class PercentageDiscount : IDiscountStrategy
{
    // TODO: constructor(double percent), store it

    public PercentageDiscount(double percent)
    {
        throw new NotImplementedException();
    }

    public double Apply(double subtotal)
    {
        throw new NotImplementedException();
    }
}

public static class Checkout
{
    // TODO: return strategy.Apply(subtotal) — no if/switch on the strategy's type
    public static double CalculateTotal(double subtotal, IDiscountStrategy strategy)
    {
        throw new NotImplementedException();
    }
}
