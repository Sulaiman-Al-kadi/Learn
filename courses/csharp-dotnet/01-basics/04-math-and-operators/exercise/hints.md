## Hint 1
Three prompt/read pairs, e.g. `Console.Write("Score 1: "); int s1 = int.Parse(Console.ReadLine() ?? "");`

## Hint 2
`sum / 3` with an `int` sum gives `85`, not `85.33`. Cast first: `(double)sum / 3`. Then `Math.Round(that, 2)`.

## Hint 3
`Math.Max` only takes two numbers. For three, nest: `Math.Max(s1, Math.Max(s2, s3))`. Same idea for `Math.Min`.

## Hint 4
`sum % 2 == 0` is already a `bool`, so `$"Sum is even: {sum % 2 == 0}"` prints `True` or `False`. Last digit is `sum % 10`.
