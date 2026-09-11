// Exercise 07 — BankAccount with custom exception (solution)

public class InsufficientFundsException : Exception
{
    public InsufficientFundsException(string message) : base(message) { }
}

public class BankAccount
{
    public double Balance { get; private set; }

    public BankAccount(double startingBalance)
    {
        Balance = startingBalance;
    }

    public void Deposit(double amount)
    {
        if (amount < 0) throw new ArgumentException("Deposit amount cannot be negative");
        Balance += amount;
    }

    public void Withdraw(double amount)
    {
        if (amount > Balance) throw new InsufficientFundsException($"Cannot withdraw {amount}, balance is only {Balance}");
        Balance -= amount;
    }
}

public static class SafeMath
{
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
}
