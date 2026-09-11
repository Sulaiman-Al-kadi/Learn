using Xunit;

public class CarTests
{
    [Fact]
    public void Constructor_SetsFieldsAndStartsMileageAtZero()
    {
        var car = new Car("Toyota", "Corolla", 2020);
        Assert.Equal("Toyota", car.Make);
        Assert.Equal("Corolla", car.Model);
        Assert.Equal(2020, car.Year);
        Assert.Equal(0, car.Mileage);
    }

    [Fact]
    public void Drive_AccumulatesMileage()
    {
        var car = new Car("Honda", "Civic", 2019);
        car.Drive(100);
        car.Drive(50);
        Assert.Equal(150, car.Mileage);
    }

    [Fact]
    public void Drive_TwoCars_AreIndependent()
    {
        var a = new Car("Kia", "Rio", 2021);
        var b = new Car("Kia", "Rio", 2021);
        a.Drive(300);
        Assert.Equal(300, a.Mileage);
        Assert.Equal(0, b.Mileage);
    }

    [Fact]
    public void Describe_FormatsCorrectly()
    {
        var car = new Car("Toyota", "Corolla", 2020);
        car.Drive(150);
        Assert.Equal("2020 Toyota Corolla - 150 miles", car.Describe());
    }

    [Fact]
    public void Describe_ZeroMileage()
    {
        var car = new Car("Ford", "Focus", 2018);
        Assert.Equal("2018 Ford Focus - 0 miles", car.Describe());
    }
}
