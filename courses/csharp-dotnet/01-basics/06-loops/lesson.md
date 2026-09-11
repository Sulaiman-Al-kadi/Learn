# 06 — Loops

Everything so far runs each line once. A **loop** repeats a block of code, either a fixed number of times or until a condition changes. This is where computers start doing things humans wouldn't have the patience for.

## `while` — repeat while a condition is true

```csharp
int count = 1;
while (count <= 5)
{
    Console.WriteLine(count);
    count++;
}
```

Output: `1 2 3 4 5`, each on its own line.

Read it as: *"check the condition. True? Run the block, then check again. False? Skip the block and continue after it."* The condition is checked **before** every run, including the first.

**The #1 loop bug: forgetting to change the thing the condition checks.**

```csharp
int count = 1;
while (count <= 5)
{
    Console.WriteLine(count);
    // forgot count++;  ← condition is ALWAYS true → infinite loop, program hangs forever
}
```

If your program seems frozen, this is almost always why. (In the terminal, `Ctrl+C` stops a hung program.)

## `do...while` — run at least once

```csharp
int n;
do
{
    Console.Write("Enter a positive number: ");
    n = int.Parse(Console.ReadLine() ?? "");
}
while (n <= 0);
```

Same as `while`, except the condition is checked **after** the block, so the block always runs at least once. Perfect for "ask again until the answer is valid."

## `for` — when you know how many times

```csharp
for (int i = 0; i < 5; i++)
{
    Console.WriteLine(i);
}
```

A `for` loop packs three things into the parentheses, separated by `;`:

| Part | Runs | Example |
|---|---|---|
| **initializer** | once, before anything | `int i = 0` — declare the loop counter |
| **condition** | before every iteration | `i < 5` — keep going while true |
| **iterator** | after every iteration | `i++` — how the counter changes |

This prints `0 1 2 3 4` — five numbers, **not including 5**, because the loop stops as soon as `i < 5` is false. Off-by-one mistakes here are extremely common — always double check whether you want `<` or `<=`.

```csharp
for (int i = 1; i <= 5; i++)   // 1 2 3 4 5  (five numbers, starting at 1)
for (int i = 5; i > 0; i--)    // 5 4 3 2 1  (counting down)
for (int i = 0; i < 10; i += 2) // 0 2 4 6 8  (step by 2)
```

The loop variable `i` (short for "index") only exists inside the loop — you can't use it after the closing `}`.

## `foreach` — walk through a collection

You'll meet real collections (arrays, lists) in the next lesson, but `foreach` deserves an early look because it's how you'll loop through them:

```csharp
int[] scores = { 90, 75, 88 };
foreach (int score in scores)
{
    Console.WriteLine(score);
}
```

Read it as: *"for each `score` in `scores`, run the block."* No counter to manage, no off-by-one risk — `foreach` is the right choice whenever you don't need the index itself, which is most of the time.

## `break` and `continue`

```csharp
for (int i = 1; i <= 10; i++)
{
    if (i == 5) break;          // stop the loop entirely, right now
    Console.WriteLine(i);
}
// prints 1 2 3 4

for (int i = 1; i <= 5; i++)
{
    if (i == 3) continue;       // skip the rest of THIS iteration, go to the next one
    Console.WriteLine(i);
}
// prints 1 2 4 5   (3 is skipped, but the loop keeps going)
```

`break` = "exit the loop now." `continue` = "skip to the next round."

## Nested loops

A loop inside a loop — common for grids, tables, pairs of things:

```csharp
for (int row = 1; row <= 3; row++)
{
    for (int col = 1; col <= 3; col++)
    {
        Console.Write($"{row * col,4}");   // ,4 = right-align in a 4-char column
    }
    Console.WriteLine();
}
```
```
   1   2   3
   2   4   6
   3   6   9
```
The outer loop runs 3 times; for each one, the *entire* inner loop runs 3 times — 9 total prints. `break`/`continue` inside a nested loop only affects the **innermost** loop they're written in.

## Accumulator pattern — building up a result

The most common loop shape: start with a starting value, update it every iteration.

```csharp
int sum = 0;
for (int i = 1; i <= 100; i++)
{
    sum += i;
}
Console.WriteLine(sum);   // 5050
```

Same idea for finding a maximum, counting matches, building a string, etc. — declare the accumulator **before** the loop, update it **inside**.

## Which loop do I pick?

| Situation | Use |
|---|---|
| Fixed number of repeats, need a counter | `for` |
| Walking through a collection's items | `foreach` |
| Repeat until some condition changes, unknown count | `while` |
| Must run at least once (e.g. "ask until valid") | `do...while` |

## Summary
- `while (cond) { }` — checks before each run.
- `do { } while (cond);` — checks after, so it runs at least once.
- `for (init; cond; step) { }` — best when you know the count.
- `foreach (var x in collection) { }` — best for walking a collection.
- `break` exits the loop; `continue` skips to the next iteration.
- Forgetting to update the loop's condition variable = infinite loop.
