// Exercise 09 — tests for Calculator
// See README.md. The test below is written but WRONG on purpose — fix it first, then add the rest.
using Xunit;

public class CalculatorTests
{
    [Fact]
    public void Add_TwoPositiveNumbers_ReturnsSum()
    {
        Assert.Equal(999, Calculator.Add(2, 3));   // <-- wrong expected value, fix this
    }

    // TODO: Subtract — a normal case

    // TODO: Divide — a normal case

    // TODO: Divide by zero — Assert.Throws<DivideByZeroException>(() => ...)

    // TODO: IsPrime — cover: a prime, a non-prime, 1 (false), 2 (true), 0 or negative (false)
    //       Either separate [Fact]s or one [Theory] with [InlineData] rows — see the lesson.
}
