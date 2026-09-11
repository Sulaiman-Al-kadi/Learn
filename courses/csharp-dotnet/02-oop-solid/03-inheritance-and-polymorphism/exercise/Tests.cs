using Xunit;

public class ShapeTests
{
    [Fact]
    public void BaseShape_AreaIsZero()
    {
        var s = new Shape();
        Assert.Equal(0, s.Area());
        Assert.Equal("Shape: area = 0", s.Describe());
    }

    [Fact]
    public void Circle_ComputesArea()
    {
        var c = new Circle(5);
        Assert.Equal(78.54, Math.Round(c.Area(), 2));
    }

    [Fact]
    public void Circle_Describe()
    {
        var c = new Circle(5);
        Assert.Equal("Circle: area = 78.54", c.Describe());
    }

    [Fact]
    public void Rectangle_ComputesArea()
    {
        var r = new Rectangle(4, 5);
        Assert.Equal(20, r.Area());
    }

    [Fact]
    public void Rectangle_Describe()
    {
        var r = new Rectangle(4, 5);
        Assert.Equal("Rectangle: area = 20", r.Describe());
    }

    [Fact]
    public void CircleAndRectangle_AreShapes()
    {
        Assert.IsAssignableFrom<Shape>(new Circle(1));
        Assert.IsAssignableFrom<Shape>(new Rectangle(1, 1));
    }

    [Fact]
    public void Polymorphism_SumOfAreasThroughBaseType()
    {
        List<Shape> shapes = new List<Shape> { new Circle(2), new Rectangle(3, 3) };
        double total = 0;
        foreach (Shape s in shapes)
        {
            total += s.Area();
        }
        Assert.Equal(Math.PI * 4 + 9, total, 5);
    }
}
