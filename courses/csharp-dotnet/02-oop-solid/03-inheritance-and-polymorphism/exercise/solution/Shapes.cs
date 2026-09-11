// Exercise 03 — Shape hierarchy (solution)

public class Shape
{
    public string Name { get; set; } = "Shape";

    public virtual double Area() => 0;

    public string Describe() => $"{Name}: area = {Math.Round(Area(), 2)}";
}

public class Circle : Shape
{
    public double Radius { get; set; }

    public Circle(double radius)
    {
        Name = "Circle";
        Radius = radius;
    }

    public override double Area() => Math.PI * Radius * Radius;
}

public class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }

    public Rectangle(double width, double height)
    {
        Name = "Rectangle";
        Width = width;
        Height = height;
    }

    public override double Area() => Width * Height;
}
