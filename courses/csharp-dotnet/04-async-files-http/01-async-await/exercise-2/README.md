# Exercise — Fetch them together

Run several tasks at once instead of one after another.

1. `SumAllAsync`: call `fetch` for every id at the same time and add up the results.
2. `FetchInOrderAsync`: same, but return the results in the order of `ids`.

The tests measure time, so awaiting each call inside a loop will fail.
