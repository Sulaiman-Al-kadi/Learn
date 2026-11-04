# Exercise — Bounded mailbox

Use a `Channel<T>` with a limit, so that a fast producer can't fill memory.

1. The constructor creates a bounded channel of `capacity` items that drops nothing: when it's full, `TryPost` returns false.
2. `TryPost`: add a message without waiting.
3. `ReadAsync`: wait for the next message.
