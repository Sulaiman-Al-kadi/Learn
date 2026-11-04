## Hint 1
Loop from 1 to `attempts`. On the last attempt, don't catch: a `catch (Exception) when (i < attempts)` filter does this neatly.

## Hint 2
An exception from an awaited task is thrown at the `await`, so the `try` must wrap the `await`.
