# Exercise — Student profile card

Build a program that stores a student's details in **variables**, then prints this card:

```
--- Student Card ---
Name:      Layla Hassan
Age:       21
GPA:       3.75
Enrolled:  True
Initial:   L
In 4 years Layla Hassan will be 25.
```

## Rules
1. Declare **five variables**, one for each of these types: `string`, `int`, `double`, `bool`, `char`. Use them for name, age, GPA, enrolled, initial.
2. Every line after the title must use **string interpolation** (`$"..."`). No `+` joining.
3. The `25` on the last line must be **computed** from the age variable (`age + 4`), not typed.
4. The alignment uses **spaces** (not tabs): after `Name:` 6 spaces, after `Age:` 7, after `GPA:` 7, after `Enrolled:` 2, after `Initial:` 3 — so every value starts in the same column. Count carefully.
5. Note that printing a `bool` gives `True` with a capital T. That's how C# prints booleans.

Check with the beaker icon / `Ctrl+Alt+Enter`. Use **Show diff** to compare.
