using Xunit;

public class BankTests
{
    [Fact]
    public void InsufficientFundsException_IsAnException()
    {
        Assert.IsAssignableFrom<Exception>(new InsufficientFundsException("test"));
    }

    [Fact]
    public void Deposit_IncreasesBalance()
    {
        var acc = new BankAccount(100);
        acc.Deposit(50);
        Assert.Equal(150, acc.Balance);
    }

    [Fact]
    public void Deposit_Negative_Throws()
    {
        var acc = new BankAccount(100);
        Assert.Throws<ArgumentException>(() => acc.Deposit(-10));
    }

    [Fact]
    public void Withdraw_WithinBalance_Decreases()
    {
        var acc = new BankAccount(100);
        acc.Withdraw(30);
        Assert.Equal(70, acc.Balance);
    }

    [Fact]
    public void Withdraw_MoreThanBalance_ThrowsAndDoesNotChangeBalance()
    {
        var acc = new BankAccount(100);
        Assert.Throws<InsufficientFundsException>(() => acc.Withdraw(150));
        Assert.Equal(100, acc.Balance);
    }

    [Fact]
    public void SafeDivide_Normal_Succeeds()
    {
        var (success, result, error) = SafeMath.SafeDivide(10, 2);
        Assert.True(success);
        Assert.Equal(5, result);
        Assert.Equal("", error);
    }

    [Fact]
    public void SafeDivide_ByZero_Fails()
    {
        var (success, result, error) = SafeMath.SafeDivide(10, 0);
        Assert.False(success);
        Assert.Equal(0, result);
        Assert.Equal("Cannot divide by zero", error);
    }
}
