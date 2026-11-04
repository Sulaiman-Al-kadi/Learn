# 07b — Memory: Values and References

So far every variable has felt like a box with a value inside. That is true for `int`, `double`, `bool`, and `char`. It is not true for everything, and the difference explains a whole family of surprising bugs. This lesson gives you the mental model. You will lean on it for the rest of the course.

## Two kinds of type

C# types come in two kinds:

| | Value type | Reference type |
|---|---|---|
| The variable holds | the value itself | the *address* of an object stored elsewhere |
| Assigning `b = a` copies | the value | the address |
| Examples | `int`, `double`, `bool`, `char`, `struct` | `string`, arrays, `List<T>`, `class` objects |

Here is what that means in code. With a value type, each variable has its own copy:

```csharp
int a = 10;
int b = a;     // b gets its own copy of 10
b = 99;
Console.WriteLine(a);   // 10 — a never changed
```

Now the same steps with an array, which is a reference type. You will study arrays properly in the next lesson; for now, `int[]` is "several ints in a row" and `[0]` means "the first one":

```csharp
int[] a = [10, 20];
int[] b = a;     // b gets a copy of the ADDRESS, so both point at one array
b[0] = 99;
Console.WriteLine(a[0]);   // 99 — a changed too!
```

There was only ever one array. `a` and `b` are two labels on it.

## Where things live: the stack and the heap

Your program's memory has two main areas.

- **The stack** holds the local variables of the method that is running. It is fast and tidy: when a method ends, its variables disappear immediately.
- **The heap** holds objects that may need to outlive a single method. Arrays, lists, strings, and class objects live here. The stack variable only holds the address.

```text
   STACK                      HEAP
 ┌────────────┐
 │ x = 5      │
 │ a = ───────┼────────►  ┌──────────┐
 │ b = ───────┼────────►  │ 99 │ 20  │
 └────────────┘           └──────────┘
```

`x` holds its value directly. `a` and `b` each hold an address, and both addresses lead to the same array.

A simplification worth knowing about: a value type stored inside a heap object lives in that object, so "value types live on the stack" isn't the full story. The rule that always holds is the one in the table: assigning a value type copies the value, and assigning a reference type copies the address.

## Passing to a method follows the same rule

A method parameter is just an assignment in disguise:

```csharp
void Change(int n, int[] nums)
{
    n = 100;        // changes the method's own copy
    nums[0] = 100;  // follows the address to the caller's array
}

int number = 1;
int[] numbers = [1, 2];
Change(number, numbers);
Console.WriteLine(number);      // 1
Console.WriteLine(numbers[0]);  // 100
```

## Comparing: same value or same object?

For value types, `==` compares values. For most reference types, `==` asks a different question: **are these the same object?** This is called **reference equality**.

```csharp
int[] a = [1, 2];
int[] b = [1, 2];
int[] c = a;

Console.WriteLine(a == b);   // False — equal contents, but two separate arrays
Console.WriteLine(a == c);   // True  — the same array
```

`object.ReferenceEquals(a, c)` asks that question explicitly, whatever the type. You will see classes behave this way in Module 2, and records change the rule on purpose.

`string` is the friendly exception: `==` on strings compares the text.

## `null`: an address that points nowhere

A reference variable can hold `null`, meaning "no object". Using one throws a `NullReferenceException`, probably the most common runtime error in .NET:

```csharp
string? name = null;
Console.WriteLine(name.Length);   // NullReferenceException at runtime
```

The `?` in `string?` says "this may be null", and the compiler warns you when you use it carelessly. That is why you keep writing `Console.ReadLine() ?? ""`: `ReadLine` can return `null`, and `??` supplies a fallback.

## Making a real copy

When you want an independent array, ask for one:

```csharp
int[] original = [1, 2, 3];
int[] copy = (int[])original.Clone();
copy[0] = 99;
Console.WriteLine(original[0]);   // 1 — untouched
```

## Strings can't change

Strings are reference types, but they are **immutable**: no operation changes an existing string. Anything that looks like a change builds a new one.

```csharp
string s = "Hi";
string t = s;
s += "!";               // builds a NEW string and points s at it
Console.WriteLine(t);   // Hi
```

So `+=` in a long loop creates a new string on every pass and throws the old one away. You will meet the fix, `StringBuilder`, two lessons from now.

## The garbage collector

You never free heap memory yourself. The **garbage collector** (GC) runs from time to time, finds objects nothing points to any more, and reclaims them. Young objects are checked most often, because most objects die young.

```csharp
int[] data = new int[1000];
data = new int[5];    // nothing points at the first array now, so the GC can reclaim it
```

## Things the GC doesn't clean up: `using`

The GC manages memory, but it knows nothing about open files or network connections. Those should be released as soon as you are done, so these types offer a `Dispose()` method, and the `using` keyword calls it for you:

```csharp
using var writer = new StreamWriter("log.txt");
writer.WriteLine("hello");
// writer.Dispose() runs automatically at the end of this block, even if an exception is thrown
```

You will use this in the files lesson. Any type that has cleanup to run implements an interface called `IDisposable`.

## Sequences can be lazy: `IEnumerable<T>`

One more idea, for later. `IEnumerable<T>` means "something you can walk through with `foreach`". An array is one. But some sequences hold no data at all: they compute each value as you walk, and they start over if you walk them twice.

```csharp
int[] nums = [1, 2, 3];
IEnumerable<int> doubled = nums.Select(n => n * 2);   // nothing has been computed yet
```

Don't worry about the syntax yet. Just keep the caution: walking such a sequence twice can do the work twice. It comes back in the LINQ module.

## Summary

- Value types: the variable holds the value, and assignment copies it.
- Reference types: the variable holds an address, and assignment copies the address.
- Method parameters follow the same rule.
- `==` on most reference types asks "same object?". `string` compares text.
- `null` means no object, and using it throws `NullReferenceException`.
- Strings are immutable, so "changing" one builds a new one.
- The GC reclaims memory; `using` releases everything else, on time.
- An `IEnumerable<T>` may recompute its values each time you walk it.
