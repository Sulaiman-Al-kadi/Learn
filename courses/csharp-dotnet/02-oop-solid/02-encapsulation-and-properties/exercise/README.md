# Exercise — Temperature class

Write a `Temperature` class in `Temperature.cs`.

## Properties
- `Celsius` — a **full property** with a private backing field. The setter must throw `new ArgumentException("Celsius cannot be below absolute zero")` if the value is less than `-273.15` (absolute zero). Otherwise it stores the value normally.
- `Fahrenheit` — a **computed, read-only** property (`=>`, no backing field, no setter) equal to `Celsius * 9 / 5 + 32`. It must always reflect the current `Celsius`, recalculated on every read.

## Constructor
`Temperature(double celsius)` — sets `Celsius` through the property (so the validation rule applies to the constructor too, not just later assignments).

## Rules
- The backing field for `Celsius` must be `private`.
- Do the validation check inside the `Celsius` setter — don't duplicate it in the constructor; the constructor should just assign `Celsius = celsius;` and let the property's own setter do the checking.
- `Fahrenheit` has no setter at all.
