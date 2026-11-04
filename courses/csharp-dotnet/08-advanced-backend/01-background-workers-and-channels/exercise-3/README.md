# Exercise — Drain the queue

Write the consumer side of a producer/consumer pair.

`RunAsync` reads every number from the channel until the producer finishes, and returns their sum.

`ProduceAsync` writes the numbers, then marks the channel complete. Without that last step, the consumer waits forever.
