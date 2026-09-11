## Hint 1
Fields are just variable declarations directly inside the class body, no method around them:
```csharp
public string Make;
public string Model;
public int Year;
public int Mileage;
```

## Hint 2
The constructor has the same name as the class and no return type. Set three fields from the three parameters, and set `Mileage = 0;` explicitly (it would default to 0 anyway, but being explicit is clearer).

## Hint 3
`Drive` is one line: `Mileage += miles;`. `Describe` is one `return` with an interpolated string — copy the exact format from the README, including the ` - ` and ` miles`.
