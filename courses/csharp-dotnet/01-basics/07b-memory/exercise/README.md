# Exercise — Aliases and copies

Read a count `n`, then `n` integers, one per line, into an `int[]` called `original`. `n` is always at least 1.

Then:

1. Make `alias`, a second variable pointing at the **same** array.
2. Make `copy`, an **independent** copy of the array.
3. Set `alias[0]` to `100`.
4. Print the five lines below.

## Output format

```
Original: <values joined with ", ">
Alias: <values joined with ", ">
Copy: <values joined with ", ">
Alias is original: <True or False>
Copy is original: <True or False>
```

## Worked example

Input:
```
3
5
6
7
```

Output:
```
Original: 100, 6, 7
Alias: 100, 6, 7
Copy: 5, 6, 7
Alias is original: True
Copy is original: False
```

## Rules

1. `int[] original = new int[n];` creates an array of `n` zeros, and `original[i] = ...` fills one slot.
2. `string.Join(", ", original)` joins an array into one string.
3. Use `object.ReferenceEquals(a, b)` for the last two lines.
4. The checker runs 3 cases, and all must pass.
