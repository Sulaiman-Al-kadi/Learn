# Exercise — Helper method library

This exercise is checked differently from the ones before it: instead of comparing printed output, a set of **unit tests** (`Tests.cs`, already written — don't edit it) calls your methods directly and checks their return values. This is exactly how real .NET projects verify code, and it's what "check": "test" means in the sidebar.

Open `Methods.cs`. Implement the five methods in the `Helpers` class. **Don't change their signatures** (name, parameter types, return type) — the tests call them exactly as declared, and a signature change means the tests won't even compile.

| Method | Does |
|---|---|
| `int Square(int n)` | returns `n * n` |
| `bool IsEven(int n)` | true if `n` is divisible by 2 |
| `int Max3(int a, int b, int c)` | the largest of the three |
| `string Repeat(string text, int times)` | `text` concatenated `times` times, no separator. `times <= 0` → `""` |
| `(int Min, int Max) MinMax(int a, int b, int c)` | smallest and largest of the three, as a tuple |

## Checking
Press the beaker / `Ctrl+Alt+Enter`. The Output panel shows which named tests failed — e.g. `Repeat_Negative` failing tells you exactly which case is wrong, which is much more precise than an output diff.

Take a look at `Tests.cs` once you're done — reading tests is a skill in itself, and it shows you exactly what "correct" means here.
