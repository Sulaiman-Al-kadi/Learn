// Exercise 02 — Temperature class (solution)

public class Temperature
{
    private double _celsius;

    public double Celsius
    {
        get => _celsius;
        set
        {
            if (value < -273.15) throw new ArgumentException("Celsius cannot be below absolute zero");
            _celsius = value;
        }
    }

    public double Fahrenheit => Celsius * 9 / 5 + 32;

    public Temperature(double celsius)
    {
        Celsius = celsius;
    }
}
