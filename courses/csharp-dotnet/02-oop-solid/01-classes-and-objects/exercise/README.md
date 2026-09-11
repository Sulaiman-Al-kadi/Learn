# Exercise — Car class

Write a `Car` class in `Car.cs`.

## Fields (public, as in the lesson)
- `string Make`
- `string Model`
- `int Year`
- `int Mileage` — starts at `0` for every new car; there's no constructor parameter for it

## Constructor
`Car(string make, string model, int year)` — sets `Make`, `Model`, `Year` from the parameters, and `Mileage` to `0`.

## Methods
- `void Drive(int miles)` — adds `miles` to `Mileage`. Calling it several times accumulates.
- `string Describe()` — returns exactly `"{Year} {Make} {Model} - {Mileage} miles"`, e.g. `"2020 Toyota Corolla - 150 miles"`.

## Rules
- Use fields (not properties yet — that's next lesson), a constructor, and instance methods, exactly like the lesson's `BankAccount` example.
- Every `Car` object must have its own independent `Mileage` — driving one car must never affect another.
