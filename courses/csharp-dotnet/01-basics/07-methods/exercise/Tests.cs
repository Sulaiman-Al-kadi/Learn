// Tests.cs — the oracle. Don't edit this; it's how "Check" decides pass/fail.
using Xunit;

public class HelpersTests
{
    [Fact] public void Square_Of4_Is16() => Assert.Equal(16, Helpers.Square(4));
    [Fact] public void Square_Of0_Is0() => Assert.Equal(0, Helpers.Square(0));
    [Fact] public void Square_OfNegative_IsPositive() => Assert.Equal(9, Helpers.Square(-3));

    [Theory]
    [InlineData(2, true)]
    [InlineData(3, false)]
    [InlineData(0, true)]
    public void IsEven_Works(int n, bool expected) => Assert.Equal(expected, Helpers.IsEven(n));

    [Fact] public void Max3_PicksLargest() => Assert.Equal(9, Helpers.Max3(3, 9, 5));
    [Fact] public void Max3_AllSame() => Assert.Equal(4, Helpers.Max3(4, 4, 4));
    [Fact] public void Max3_LargestFirst() => Assert.Equal(7, Helpers.Max3(7, 2, 1));

    [Fact] public void Repeat_Basic() => Assert.Equal("hahaha", Helpers.Repeat("ha", 3));
    [Fact] public void Repeat_Zero() => Assert.Equal("", Helpers.Repeat("x", 0));
    [Fact] public void Repeat_Negative() => Assert.Equal("", Helpers.Repeat("x", -2));
    [Fact] public void Repeat_One() => Assert.Equal("go", Helpers.Repeat("go", 1));

    [Fact]
    public void MinMax_Basic()
    {
        var (min, max) = Helpers.MinMax(5, 2, 9);
        Assert.Equal(2, min);
        Assert.Equal(9, max);
    }

    [Fact]
    public void MinMax_AllSame()
    {
        var (min, max) = Helpers.MinMax(4, 4, 4);
        Assert.Equal(4, min);
        Assert.Equal(4, max);
    }
}
