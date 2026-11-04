# Exercise — Structured logging

Log with the right level, and with named placeholders rather than string interpolation.

`Charge(user, amount)`:

- amount above 0: log `Information` with the template `Charged {User} {Amount}`, and return true
- otherwise: log `Warning` with the template `Rejected charge for {User}`, and return false

The tests use a fake `ILogger` that records the template, not the final text. `$"..."` strings will fail.
