// Exercise 02 — Temperature class
// See README.md. A validated full property, a computed property, and a constructor.

public class Temperature
{
    // TODO: private backing field for Celsius
    private double _celsius;

    // TODO: full property — get returns _celsius; set validates then assigns _celsius
    public double Celsius
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    // TODO: computed, read-only — => Celsius * 9 / 5 + 32
    public double Fahrenheit => throw new NotImplementedException();

    public Temperature(double celsius)
    {
        throw new NotImplementedException();
    }
}
