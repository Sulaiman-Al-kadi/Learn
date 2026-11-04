# Exercise — Retry a flaky call

Real calls fail sometimes. Handle that with `try`/`catch` around `await`.

1. `RetryAsync`: call `action` up to `attempts` times. Return the first success. If every attempt fails, let the last exception escape.
2. `OrDefaultAsync`: return the result of `action`, or `fallback` if it throws.
