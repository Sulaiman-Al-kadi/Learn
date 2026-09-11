# Exercise — BankAccount with custom exception

Write everything in `Bank.cs`.

## `InsufficientFundsException : Exception`
A custom exception with a constructor `InsufficientFundsException(string message) : base(message)`.

## `BankAccount`
- `double Balance { get; private set; }`
- Constructor `BankAccount(double startingBalance)` — sets `Balance`.
- `void Deposit(double amount)` — if `amount < 0`, `throw new ArgumentException("Deposit amount cannot be negative")`. Otherwise add it to `Balance`.
- `void Withdraw(double amount)` — if `amount > Balance`, `throw new InsufficientFundsException($"Cannot withdraw {amount}, balance is only {Balance}")`. Otherwise subtract it from `Balance`.

## `SafeMath` (static class)
```csharp
public static (bool Success, double Result, string Error) SafeDivide(double a, double b)
```
Attempt `a / b` inside a `try`. If `b` is `0`, `throw new DivideByZeroException()` yourself first (don't rely on floating-point division producing infinity — actively check and throw), then `catch` it and return `(false, 0, "Cannot divide by zero")`. On success, return `(true, a / b, "")`.

## Rules
- `Withdraw` must not change `Balance` at all when it throws.
- `SafeDivide` must genuinely use `try`/`catch` internally — the point of the exercise is practicing the mechanism, even though a simple `if` alone could also solve it.
