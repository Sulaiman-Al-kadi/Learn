# Exercise — Number stats + multiplication table

**Part 1 — read numbers until a sentinel.** Read whole numbers one per line. `0` means "stop" and is **not** included in the stats. You don't know in advance how many numbers there will be — that's a `while` loop, not a `for`.

**Part 2 — after the numbers, read one more line**: an integer `n`, the size of a multiplication table to print (rows and columns `1..n`).

## Output format

```
Numbers: <count read>
Sum: <total>
Average: <sum/count, rounded to 2 decimals>
Max: <largest>
Min: <smallest>
Evens: <how many were even>
Table:
<n rows, each number right-aligned in a 4-character column>
```

If `count` is 0 (the very first number was `0`), print `Numbers: 0` and then **skip** the Sum/Average/Max/Min/Evens lines entirely (there's nothing to compute), go straight to `Table:`.

## Worked example
Input:
```
5
3
8
2
0
3
```
(four numbers `5,3,8,2`, then `0` stops the loop, then `3` is the table size)

Output:
```
Numbers: 4
Sum: 18
Average: 4.5
Max: 8
Min: 2
Evens: 2
Table:
   1   2   3
   2   4   6
   3   6   9
```

## Rules
1. Use a `while` loop for Part 1 (you don't know the count ahead of time). Use `int.Parse` on each line.
2. Track `count`, `sum`, `max`, `min`, `evens` as accumulator variables, updated **inside** the loop. Use `Math.Max` / `Math.Min` to update the running max/min — don't write your own `if` for it (though that would also work, `Math.Max`/`Math.Min` is the idiom).
3. Average: watch the integer-division trap from lesson 04. Round to 2 decimals.
4. Use a **nested `for` loop** for the table. `{value,4}` right-aligns a value in a 4-character column (you used this in lesson 06's notes).
5. The checker runs 4 cases in `cases/`. All must pass.
