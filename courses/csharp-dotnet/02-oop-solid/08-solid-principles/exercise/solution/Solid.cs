// Exercise 08 — DIP notifier + OCP discount strategy (solution)

public interface INotifier
{
    void Send(string message);
}

public class OrderProcessor
{
    private readonly INotifier _notifier;

    public OrderProcessor(INotifier notifier)
    {
        _notifier = notifier;
    }

    public void Complete(string orderId)
    {
        _notifier.Send($"Order {orderId} completed");
    }
}

public interface IDiscountStrategy
{
    double Apply(double subtotal);
}

public class NoDiscount : IDiscountStrategy
{
    public double Apply(double subtotal) => subtotal;
}

public class PercentageDiscount : IDiscountStrategy
{
    private readonly double _percent;

    public PercentageDiscount(double percent)
    {
        _percent = percent;
    }

    public double Apply(double subtotal) => subtotal - subtotal * _percent / 100;
}

public static class Checkout
{
    public static double CalculateTotal(double subtotal, IDiscountStrategy strategy)
    {
        return strategy.Apply(subtotal);
    }
}
