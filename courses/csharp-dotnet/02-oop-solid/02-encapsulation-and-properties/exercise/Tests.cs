using Xunit;

public class TemperatureTests
{
    [Fact]
    public void Freezing_ComputesFahrenheitCorrectly()
    {
        var t = new Temperature(0);
        Assert.Equal(0, t.Celsius);
        Assert.Equal(32, t.Fahrenheit);
    }

    [Fact]
    public void Boiling_ComputesFahrenheitCorrectly()
    {
        var t = new Temperature(100);
        Assert.Equal(212, t.Fahrenheit);
    }

    [Fact]
    public void SettingCelsius_UpdatesFahrenheitAutomatically()
    {
        var t = new Temperature(0);
        t.Celsius = 20;
        Assert.Equal(68, t.Fahrenheit);
    }

    [Fact]
    public void ConstructorBelowAbsoluteZero_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Temperature(-300));
    }

    [Fact]
    public void SettingBelowAbsoluteZero_Throws()
    {
        var t = new Temperature(0);
        Assert.Throws<ArgumentException>(() => t.Celsius = -274);
    }

    [Fact]
    public void ExactlyAbsoluteZero_IsAllowed()
    {
        var t = new Temperature(-273.15);
        Assert.Equal(-273.15, t.Celsius);
    }
}
