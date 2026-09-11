# 01 — Printing

Every program you will ever write does one thing at its core: it takes some input, does something with it, and shows a result. Today we learn the last part — **showing a result** on the screen.

## Your first line of C#

```csharp
Console.WriteLine("Hello, world!");
```

Run that and the screen shows:

```
Hello, world!
```

Let's take that line apart, piece by piece. Every piece matters.

| Piece | What it is |
|---|---|
| `Console` | The **console** is the black text window your program runs in. `Console` is the name of a built-in toolbox for talking to it. |
| `.` | The dot means *"look inside"*. `Console.WriteLine` = "the `WriteLine` tool that is inside the `Console` toolbox". |
| `WriteLine` | A **method** — a named action the computer can perform. This one writes text and then moves to a new line. |
| `( )` | Parentheses **run** the method. Whatever you put inside them is what you're giving it to work with. |
| `"Hello, world!"` | Text. In C#, text is called a **string**, and it must always be wrapped in double quotes `"`. |
| `;` | The semicolon ends the **statement** (one complete instruction). Every instruction in C# ends with `;`. Forgetting it is the most common beginner error — and the compiler will tell you. |

> **Compiler?** When you run a C# file, a program called the *compiler* first reads all your text and translates it into instructions the computer understands. If it finds a mistake — a missing `;`, a misspelled word — it stops and tells you the line number. That's not a punishment; it's the compiler helping you before anything runs.

## `WriteLine` vs `Write`

There are two ways to print:

```csharp
Console.Write("A");
Console.Write("B");
Console.WriteLine("C");
Console.WriteLine("D");
```

Before reading on: what do you think this prints? Take a guess.

The output is:

```
ABC
D
```

- `Write` prints and **stays on the same line**.
- `WriteLine` prints and **then moves to the next line** (the "Line" part = "and end the line").

So `A`, `B`, `C` land together, and only after `C` does the cursor move down. Then `D` starts on a fresh line.

`Console.WriteLine();` with nothing inside prints an empty line — useful for spacing.

## Printing numbers and math

Text needs quotes. Numbers don't:

```csharp
Console.WriteLine(42);         // prints 42
Console.WriteLine(10 + 5);     // prints 15  ← C# does the math first, then prints the result
Console.WriteLine("10 + 5");   // prints 10 + 5  ← it's in quotes, so it's just text
```

This distinction — *text vs. a value the computer computes* — is one of the most important ideas in programming. Quotes mean "don't think, just show these characters".

## Special characters inside strings — escape sequences

What if you want to print a quote mark? `"He said "hi""` would confuse the compiler — it thinks the string ended at the second `"`.

The solution is the **backslash `\`**, which means *"the next character is special"*:

| You write | You get | Name |
|---|---|---|
| `\"` | `"` | escaped quote |
| `\\` | `\` | escaped backslash |
| `\n` | (a new line) | newline |
| `\t` | (a tab — a big space) | tab |

```csharp
Console.WriteLine("He said \"hi\"");     // He said "hi"
Console.WriteLine("Line 1\nLine 2");     // two lines
Console.WriteLine("Name:\tAhmed");       // Name:    Ahmed
Console.WriteLine("C:\\Users\\ahmed");   // C:\Users\ahmed
```

**Trap:** on Windows, paths have backslashes. `"C:\Learn"` is an **error** because `\L` isn't a known escape. Write `"C:\\Learn"`.

A shortcut for paths: put `@` before the string and backslashes become normal characters:

```csharp
Console.WriteLine(@"C:\Learn\courses");   // works — this is a "verbatim" string
```

## Comments

```csharp
// This is a comment. The compiler ignores everything after the two slashes.
Console.WriteLine("Hi");   // comments can also go at the end of a line
```

Comments are notes for humans. Use them to explain *why* you did something, not to repeat what the code obviously does.

## How to run a C# file

Open a terminal in VS Code (`` Ctrl+` ``) and type:

```
dotnet run path/to/Program.cs
```

.NET 10 lets you run a single `.cs` file directly like this. Later, we'll use *projects* (folders with many files), but for now one file is all you need.

## Summary

- `Console.WriteLine("text");` prints text and moves to the next line.
- `Console.Write("text");` prints and stays on the line.
- Text goes in `"double quotes"`. Numbers and math don't.
- Every statement ends with `;`.
- `\n` newline, `\t` tab, `\"` quote, `\\` backslash. Or use `@"..."` for paths.
- `//` starts a comment.

Now take the quiz, then do the exercise.
