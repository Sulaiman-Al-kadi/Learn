# 08 — Arrays & Lists

A single variable holds one value. A **collection** holds many, under one name. This lesson covers the two you'll use constantly: the fixed-size **array** and the growable **`List<T>`**.

## Arrays — fixed size, known up front

```csharp
int[] scores = { 90, 75, 88, 60 };
```

| Piece | Meaning |
|---|---|
| `int[]` | "array of `int`" — the `[]` after the type means "a collection of these." |
| `scores` | the name |
| `{ 90, 75, 88, 60 }` | an **array initializer** — the starting values |

You can also create one with a fixed size and no values (all `int`s default to `0`, all `string`s to `null`):

```csharp
int[] grid = new int[5];     // 5 ints, all 0: [0, 0, 0, 0, 0]
```

### Indexing — accessing one element

Elements are numbered starting at **0**:

```csharp
int[] scores = { 90, 75, 88, 60 };
Console.WriteLine(scores[0]);   // 90  ← the FIRST element
Console.WriteLine(scores[3]);   // 60  ← the LAST element (index = count - 1)
scores[1] = 100;                // change an element
```

`scores.Length` gives the count (4 here). **`scores[4]`** would throw an `IndexOutOfRangeException` — there is no 5th element, the valid indices are `0..3`. This is the single most common collection bug. `scores[scores.Length - 1]` is always the last element.

### Looping through an array

```csharp
for (int i = 0; i < scores.Length; i++)
{
    Console.WriteLine($"scores[{i}] = {scores[i]}");
}

// or, when you don't need the index:
foreach (int s in scores)
{
    Console.WriteLine(s);
}
```

Use `for` when you need the **position** (to compare neighbors, to write back into the array). Use `foreach` when you just need each **value**.

### Arrays are a fixed size — forever

```csharp
int[] a = new int[3];
a[3] = 1;     // ERROR at runtime: IndexOutOfRangeException — there is no growing an array
```

If you don't know the count ahead of time, or it will change, you don't want an array — you want a `List<T>`.

## `List<T>` — a growable collection

```csharp
List<int> scores = new List<int>();   // starts empty
scores.Add(90);
scores.Add(75);
scores.Add(88);
Console.WriteLine(scores.Count);      // 3   ← Count, not Length!
```

You can also use a collection expression, exactly like arrays:

```csharp
List<string> names = ["Ali", "Sara", "Omar"];
```

`T` is a **placeholder** for whatever type you're storing — `List<int>`, `List<string>`, `List<double>` are all different, specific lists. This is called a **generic type**; you'll see much more of it later.

### The methods that make `List<T>` worth using

```csharp
List<int> nums = [10, 20, 30];

nums.Add(40);                  // [10, 20, 30, 40]
nums.Insert(0, 5);              // [5, 10, 20, 30, 40]  — insert at a position
nums.Remove(20);                 // [5, 10, 30, 40]      — removes the FIRST 20 found
nums.RemoveAt(0);                 // [10, 30, 40]          — removes by position
Console.WriteLine(nums.Contains(30));  // True
Console.WriteLine(nums.IndexOf(30));   // 1   (-1 if not found)
nums.Sort();                             // sorts in place, ascending
nums.Reverse();                          // reverses in place
nums.Clear();                            // empty it out
```

Indexing (`nums[0]`), `foreach`, and `Count` all work exactly like arrays. The difference is `List<T>` can grow and shrink; an array cannot.

### Array or List — which one?

| | Array `T[]` | `List<T>` |
|---|---|---|
| Size | fixed forever | grows/shrinks freely |
| Declare | `int[] a = [1,2,3];` | `List<int> a = [1,2,3];` |
| Count | `.Length` | `.Count` |
| Add/remove | not possible | `.Add()`, `.Remove()`, `.RemoveAt()` |

**Default to `List<T>`.** Reach for a plain array only when the size is truly fixed and known (e.g. "the 7 days of the week").

## 2D arrays — a grid (brief preview)

```csharp
int[,] grid = new int[2, 3];    // 2 rows, 3 columns, all 0
grid[0, 0] = 1;
grid[1, 2] = 9;

for (int r = 0; r < 2; r++)
{
    for (int c = 0; c < 3; c++)
        Console.Write($"{grid[r, c],3}");
    Console.WriteLine();
}
```
You won't need these often — `List<List<T>>` is more common when the row sizes can differ — but recognize the syntax.

## Converting between them

```csharp
int[] arr = [1, 2, 3];
List<int> list = arr.ToList();     // array → list
int[] backToArray = list.ToArray(); // list → array
```

## A note on reference semantics

Arrays and lists are **reference types** (from Module 1's memory notes) — assigning one to another variable copies the *reference*, not the contents:

```csharp
int[] a = [1, 2, 3];
int[] b = a;          // b points to the SAME array
b[0] = 99;
Console.WriteLine(a[0]);   // 99 — a changed too!
```

To get an independent copy: `int[] b = (int[])a.Clone();` or `List<int> copy = new List<int>(original);`.

## Summary
- Arrays: fixed size, `T[]`, `.Length`.
- `List<T>`: growable, `.Count`, `.Add`/`.Remove`/`.RemoveAt`/`.Contains`/`.Sort`.
- Indexing starts at `0`; the last valid index is `count - 1`.
- Default to `List<T>` unless the size is truly fixed.
- Both are reference types — copying the variable doesn't copy the data.
