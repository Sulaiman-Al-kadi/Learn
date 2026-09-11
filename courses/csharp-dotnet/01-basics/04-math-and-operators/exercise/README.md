# Exercise — Grade statistics

Read three exam scores (whole numbers), then print statistics about them.

The checker types these three lines (`input.txt`):
```
88
73
95
```

Expected output (remember: prompts land on one line because typed input isn't echoed):
```
Score 1: Score 2: Score 3: 
Sum: 256
Average: 85.33
Highest: 95
Lowest: 73
Range: 22
Sum is even: True
Last digit of sum: 6
```

## Rules
1. Prompts are `Score 1: `, `Score 2: `, `Score 3: ` (with the trailing space), read with `int.Parse`.
2. After the three prompts, `Console.WriteLine();` once.
3. `Average` must be a **real** division (85.33, not 85). Watch out for integer division! Round it to 2 decimal places with `Math.Round(value, 2)`.
4. `Highest` / `Lowest` — use `Math.Max` and `Math.Min` (you'll need to nest them: max of three = `Math.Max(a, Math.Max(b, c))`).
5. `Range` = highest − lowest.
6. `Sum is even` — print the result of a `%` comparison directly (it's a `bool`, so it prints `True`/`False`).
7. `Last digit of sum` — use `%`.

Everything must be **computed** from the three variables. No hard-coded numbers except `2`, `10`, and the `2` in `Math.Round`.
