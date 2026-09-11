// Exercise 07 — BankAccount with custom exception
// See README.md.

public class InsufficientFundsException : Exception
{
    // TODO: constructor(string message) : base(message)
    public InsufficientFundsException(string message) : base(message)
    {
    }
}

public class BankAccount
{
    // TODO: Balance property, constructor, Deposit, Withdraw
    public double Balance { get; private set; }

    public BankAccount(double startingBalance)
    {
        throw new NotImplementedException();
    }

    public void Deposit(double amount)
    {
        throw new NotImplementedException();
    }

    public void Withdraw(double amount)
    {
        throw new NotImplementedException();
    }
}

public static class SafeMath
{
    // TODO: try/catch around the division; throw DivideByZeroException yourself when b == 0
    public static (bool Success, double Result, string Error) SafeDivide(double a, double b)
    {
        throw new NotImplementedException();
    }
}
