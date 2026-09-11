// Exercise 03 — Shape hierarchy
// See README.md. One virtual method on the base, overridden by each derived class.

public class Shape
{
    public string Name { get; set; } = "Shape";

    // TODO: virtual double Area() => 0;
    public virtual double Area() => throw new NotImplementedException();

    // TODO: string Describe() => $"{Name}: area = {Math.Round(Area(), 2)}";  (not virtual!)
    public string Describe() => throw new NotImplementedException();
}

public class Circle : Shape
{
    // TODO: double Radius { get; set; }

    // TODO: constructor Circle(double radius) — Name = "Circle", Radius = radius
    public Circle(double radius)
    {
        throw new NotImplementedException();
    }

    // TODO: override double Area() => Math.PI * Radius * Radius;
}

public class Rectangle : Shape
{
    // TODO: double Width { get; set; }, double Height { get; set; }

    // TODO: constructor Rectangle(double width, double height) — Name = "Rectangle", Width, Height
    public Rectangle(double width, double height)
    {
        throw new NotImplementedException();
    }

    // TODO: override double Area() => Width * Height;
}
