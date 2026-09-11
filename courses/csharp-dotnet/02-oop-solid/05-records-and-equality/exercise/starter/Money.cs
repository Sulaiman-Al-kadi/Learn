// Exercise 05 — Money record
// See README.md. Start from the positional record declaration, then add two methods using `with`.

public record Money(decimal Amount, string Currency)
{
    // TODO: Money Add(Money other) — throw ArgumentException("Currency mismatch") if currencies differ,
    //       otherwise return `this with { Amount = ... }`
    public Money Add(Money other)
    {
        throw new NotImplementedException();
    }

    // TODO: Money ApplyDiscount(decimal percent) — return `this with { Amount = ... }` reduced by percent%
    public Money ApplyDiscount(decimal percent)
    {
        throw new NotImplementedException();
    }
}
