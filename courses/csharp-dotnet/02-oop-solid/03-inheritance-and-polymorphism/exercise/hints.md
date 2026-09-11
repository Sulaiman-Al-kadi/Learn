## Hint 1
`Shape.Area()` in the base class: `public virtual double Area() => 0;`. `Describe` is a normal (non-virtual) method that CALLS the virtual `Area()` — that indirection is what makes it automatically correct for every subclass:
```csharp
public string Describe() => $"{Name}: area = {Math.Round(Area(), 2)}";
```

## Hint 2
Each derived class needs its own properties (`Radius`, or `Width`+`Height`), a constructor that sets `Name` to the right string plus its own properties, and: `public override double Area() => ...;` with the actual formula.

## Hint 3
Don't forget `: Shape` after the class name for both `Circle` and `Rectangle` — that's the inheritance itself. Without `override` (and `virtual` on the base), `Describe()` would always report `0` no matter what shape it's called on.
