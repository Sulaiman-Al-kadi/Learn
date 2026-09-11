// Exercise 01 — Car class (solution)

public class Car
{
    public string Make;
    public string Model;
    public int Year;
    public int Mileage;

    public Car(string make, string model, int year)
    {
        Make = make;
        Model = model;
        Year = year;
        Mileage = 0;
    }

    public void Drive(int miles)
    {
        Mileage += miles;
    }

    public string Describe()
    {
        return $"{Year} {Make} {Model} - {Mileage} miles";
    }
}
