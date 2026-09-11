## Hint 1
`Square` and `IsEven` are one-liners: `return n * n;` and `return n % 2 == 0;`.

## Hint 2
`Max3` — you don't need an `if` chain. `Math.Max` takes two values; nest it: `Math.Max(a, Math.Max(b, c))`. Same idea with `Math.Min` for the `MinMax` method.

## Hint 3
`Repeat` — handle the `times <= 0` case first with an early `return "";`. Then build the result in a loop: start with `""`, and `+=` `text` onto it `times` times (lesson 06's accumulator pattern, but for strings).

## Hint 4
`MinMax` returns a **tuple**: `return (min, max);` where the method's return type is `(int Min, int Max)`. Compute `min` and `max` first (see Hint 2), then return them together.
