# Exercise — List toolkit

Read a count `n`, then `n` integers (one per line) into a `List<int>`. Then print five derived views of that list. **Don't use LINQ** (`Where`, `OrderBy`, etc.) — you haven't learned it yet; do this with loops and `List<T>` methods, which is the point of the exercise.

## Output format

```
Original: <comma-space joined, original order>
Sorted: <ascending>
Reversed: <original order, reversed>
Unique: <first occurrence of each value, in original order, no duplicates>
Second largest: <the second-largest DISTINCT value, or "none" if there isn't one>
```

## Worked example
Input:
```
6
5
3
5
8
1
3
```
Output:
```
Original: 5, 3, 5, 8, 1, 3
Sorted: 1, 3, 3, 5, 5, 8
Reversed: 3, 1, 8, 5, 3, 5
Unique: 5, 3, 8, 1
Second largest: 5
```
(Distinct values are 8, 5, 3, 1 → largest is 8, second-largest is 5.)

If after removing duplicates there's only **one** distinct value (or zero), print `Second largest: none`.

## Rules
1. `string.Join(", ", someList)` turns a `List<int>` into a comma-separated string directly — no conversion needed.
2. **Sorted**: copy the list first (`new List<int>(original)`), then `.Sort()` the copy — don't sort the original, you still need its order for "Reversed".
3. **Reversed**: same idea with `.Reverse()` on a copy.
4. **Unique**: walk the original with a `foreach`; keep a result list; only `.Add()` a value if the result list doesn't already `.Contain()` it.
5. **Second largest**: work it out from the *Unique* list with a loop that tracks the largest and second-largest seen so far (see lesson 06's accumulator pattern) — don't sort-and-index, it's a good exercise to do it in one pass.
6. The checker runs 4 cases. All must pass.
