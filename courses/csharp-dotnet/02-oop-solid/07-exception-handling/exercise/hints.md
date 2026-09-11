## Hint 1
`InsufficientFundsException`'s constructor body can be empty — all the work is `: base(message)`, which the starter already has.

## Hint 2
`BankAccount`'s constructor: `Balance = startingBalance;`. `Deposit` and `Withdraw` each start with a guard clause (an `if` that throws), then do the actual math — this mirrors the `Temperature` property setter from lesson 02.

## Hint 3
```csharp
public static (bool Success, double Result, string Error) SafeDivide(double a, double b)
{
    try
    {
        if (b == 0) throw new DivideByZeroException();
        return (true, a / b, "");
    }
    catch (DivideByZeroException)
    {
        return (false, 0, "Cannot divide by zero");
    }
}
```
