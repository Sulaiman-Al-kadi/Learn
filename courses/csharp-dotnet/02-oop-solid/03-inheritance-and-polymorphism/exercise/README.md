# Exercise — Shape hierarchy

Write three classes in `Shapes.cs`: a base `Shape` and two derived types, `Circle` and `Rectangle`.

## `Shape` (base class)
- `string Name { get; set; }` — auto-property, defaults to `"Shape"`.
- `virtual double Area()` — returns `0` in the base class (a generic `Shape` has no defined area).
- `string Describe()` — **not** virtual, returns `$"{Name}: area = {Math.Round(Area(), 2)}"`. Because it calls `Area()` (which IS virtual), it automatically uses whichever override the actual object has — you don't need to touch `Describe` in the derived classes at all.

## `Circle : Shape`
- `double Radius { get; set; }`
- Constructor `Circle(double radius)` — sets `Name = "Circle"` and `Radius = radius`.
- `override double Area()` — `Math.PI * Radius * Radius`.

## `Rectangle : Shape`
- `double Width { get; set; }`, `double Height { get; set; }`
- Constructor `Rectangle(double width, double height)` — sets `Name = "Rectangle"`, `Width`, `Height`.
- `override double Area()` — `Width * Height`.

## Rules
- `Area()` must be `virtual` on `Shape` and `override` on both derived classes — that's what makes `Describe()` automatically correct for every shape without any changes to `Describe()` itself. This is the entire point of the lesson: polymorphism through one virtual method.
- Don't override `Describe()` anywhere — write it once, on `Shape`.
