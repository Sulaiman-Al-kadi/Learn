// Exercise 05 — Money record (solution)

public record Money(decimal Amount, string Currency)
{
    public Money Add(Money other)
    {
        if (other.Currency != Currency) throw new ArgumentException("Currency mismatch");
        return this with { Amount = Amount + other.Amount };
    }

    public Money ApplyDiscount(decimal percent)
    {
        return this with { Amount = Amount - Amount * percent / 100 };
    }
}
