## Hint 1
The backing field and property pair look like this:
```csharp
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
```

## Hint 2
`Fahrenheit` has no field at all — it's computed fresh every time: `public double Fahrenheit => Celsius * 9 / 5 + 32;`

## Hint 3
The constructor body is one line: `Celsius = celsius;` — because that goes through the property's `set`, the validation already applies without writing it twice.
