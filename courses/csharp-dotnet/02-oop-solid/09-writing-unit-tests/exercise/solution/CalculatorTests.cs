// Exercise 09 — tests for Calculator (solution)
using Xunit;

public class CalculatorTests
{
    [Fact]
    public void Add_TwoPositiveNumbers_ReturnsSum()
    {
        Assert.Equal(5, Calculator.Add(2, 3));
    }

    [Fact]
    public void Subtract_ReturnsDifference()
    {
        Assert.Equal(1, Calculator.Subtract(4, 3));
    }

    [Fact]
    public void Divide_NormalCase_ReturnsQuotient()
    {
        Assert.Equal(5, Calculator.Divide(10, 2));
    }

    [Fact]
    public void Divide_ByZero_ThrowsDivideByZeroException()
    {
        Assert.Throws<DivideByZeroException>(() => Calculator.Divide(10, 0));
    }

    [Theory]
    [InlineData(7, true)]
    [InlineData(8, false)]
    [InlineData(1, false)]
    [InlineData(2, true)]
    [InlineData(0, false)]
    [InlineData(-5, false)]
    public void IsPrime_Works(int number, bool expected)
    {
        Assert.Equal(expected, Calculator.IsPrime(number));
    }
}
