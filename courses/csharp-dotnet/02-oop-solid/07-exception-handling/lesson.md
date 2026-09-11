# Module 2, Lesson 07 — Exception Handling

You've seen crashes throughout this course: `int.Parse("abc")`, dividing badly, `IndexOutOfRangeException`. These are **exceptions** — .NET's mechanism for "something went wrong and normal execution cannot continue." This lesson is about handling them deliberately instead of letting them crash your program.

## What happens when an exception is thrown, unhandled

```csharp
int[] arr = { 1, 2, 3 };
Console.WriteLine(arr[10]);     // throws IndexOutOfRangeException
Console.WriteLine("done");      // never runs — the program terminates immediately
```

An unhandled exception stops the program right there, prints a stack trace, and skips everything after it. Every method that was in the middle of running gets abandoned too — this is called **unwinding the stack**.

## `try` / `catch` — handling it

```csharp
try
{
    int[] arr = { 1, 2, 3 };
    Console.WriteLine(arr[10]);
}
catch (IndexOutOfRangeException ex)
{
    Console.WriteLine($"That index doesn't exist: {ex.Message}");
}

Console.WriteLine("Program continues normally.");
```

| Block | Runs |
|---|---|
| `try { ... }` | The code that might throw. If it throws, execution jumps immediately to a matching `catch`. |
| `catch (ExceptionType ex) { ... }` | Only runs if an exception of that type (or a subtype of it) was thrown inside the `try`. `ex` holds the exception's details — `ex.Message` is a human-readable description. |

Code after the `try`/`catch` block runs normally afterward — the crash was contained.

## Catching specific types — order matters

```csharp
try
{
    int n = int.Parse(Console.ReadLine() ?? "");
    int result = 100 / n;
    Console.WriteLine(result);
}
catch (FormatException)
{
    Console.WriteLine("That wasn't a valid number.");
}
catch (DivideByZeroException)
{
    Console.WriteLine("Can't divide by zero.");
}
catch (Exception ex)
{
    Console.WriteLine($"Something else went wrong: {ex.Message}");
}
```

Like `if`/`else if` chains (Module 1), `catch` blocks are checked **top to bottom**, and the first matching one runs. `Exception` is the base type that **every** exception inherits from — a `catch (Exception ex)` at the end catches anything not already caught above. Put specific catches **before** general ones; the reverse order means the general one always wins and the specific ones are unreachable (the compiler will warn you).

You don't have to name the exception variable if you don't use it: `catch (FormatException) { ... }` is valid.

## `finally` — always runs

```csharp
try
{
    Console.WriteLine("Trying...");
    throw new InvalidOperationException("oops");
}
catch (InvalidOperationException)
{
    Console.WriteLine("Caught it.");
}
finally
{
    Console.WriteLine("This always runs — exception or not.");
}
```

`finally` runs whether the `try` succeeded, threw and was caught, or even if it threw something **not** caught here (it still runs on the way out, before the exception propagates further). It's for cleanup that must happen no matter what — closing a file, releasing a resource. (You already have a cleaner tool for that specific case, though: `using`, from Module 1's memory lesson — it's built on exactly this mechanism.)

## `throw` — raising your own exception

You're not limited to exceptions the runtime generates — raise your own to signal "this input/state is invalid":

```csharp
public double Divide(double a, double b)
{
    if (b == 0)
    {
        throw new ArgumentException("Cannot divide by zero");
    }
    return a / b;
}
```

Common built-in exception types to reach for:

| Type | When |
|---|---|
| `ArgumentException` | An argument's value is wrong for this method |
| `ArgumentNullException` | An argument was unexpectedly `null` |
| `ArgumentOutOfRangeException` | An argument is outside the allowed range (e.g. negative age) |
| `InvalidOperationException` | The object's current state doesn't allow this operation (e.g. withdraw from a closed account) |
| `NotImplementedException` | You've seen this all course — a placeholder for unfinished code |

Prefer these specific types over the generic `Exception` — it lets callers catch precisely what they expect and tells the next reader exactly what went wrong.

## Custom exception types

For domain-specific errors, define your own by inheriting from `Exception`:

```csharp
public class InsufficientFundsException : Exception
{
    public InsufficientFundsException(string message) : base(message) { }
}

public class BankAccount
{
    public double Balance { get; private set; }

    public void Withdraw(double amount)
    {
        if (amount > Balance)
        {
            throw new InsufficientFundsException($"Cannot withdraw {amount}, balance is only {Balance}");
        }
        Balance -= amount;
    }
}
```

```csharp
try
{
    account.Withdraw(1000);
}
catch (InsufficientFundsException ex)
{
    Console.WriteLine(ex.Message);
}
```

`: base(message)` (lesson 03!) passes the message up to `Exception`'s own constructor, which is what makes `ex.Message` work.

## When to throw vs. return a "did it work" signal

Lesson 09's `int.TryParse` returns `bool` instead of throwing — that's deliberate, because "the text wasn't a number" is an **expected, routine** outcome when reading user input. Reserve exceptions for genuinely **exceptional** situations — things that shouldn't normally happen, or that the immediate caller can't reasonably continue past without deciding what to do. A method like `Withdraw` throwing on insufficient funds is reasonable because that's a real business rule violation, not routine control flow.

## Don't swallow exceptions silently

```csharp
try
{
    DoSomething();
}
catch (Exception)
{
    // empty — the error just vanishes, and you'll have no idea anything went wrong
}
```

This is one of the worst habits in real code: it hides bugs instead of fixing them. At minimum, log or print what happened. Only catch what you can actually do something sensible about.

## Summary
- `try { risky } catch (SpecificException ex) { handle }` — contains a crash instead of terminating the program.
- Multiple `catch` blocks are checked top to bottom; put specific exception types before general ones.
- `finally { }` always runs, whether or not an exception occurred.
- `throw new SomeException("message");` raises your own exception. Prefer specific built-in types (`ArgumentException`, `InvalidOperationException`, ...) over generic `Exception`.
- Custom exceptions: `class MyException : Exception { public MyException(string msg) : base(msg) { } }`.
- Exceptions are for exceptional/invalid situations, not routine "did this work?" checks (use `TryParse`-style patterns for those).
- Never catch-and-ignore silently.
