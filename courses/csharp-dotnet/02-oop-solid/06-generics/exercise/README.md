# Exercise — Generic stack + Find

Write two things in `Generics.cs`.

## `MyStack<T>` (generic class)
(Named `MyStack` rather than `Stack` so it doesn't clash with .NET's built-in one.)
- `void Push(T item)` — adds to the top.
- `T Pop()` — removes and returns the top item (last one pushed).
- `T Peek()` — returns the top item **without** removing it.
- `int Count { get; }` — how many items are currently in the stack.

Store the items internally however you like (a `List<T>` is the natural choice — treat the end of the list as "the top").

## `GenericTools` (static class) with one generic method
```csharp
public static T? Find<T>(List<T> items, Func<T, bool> predicate)
```
Walk `items` with a `foreach`; return the **first** item for which `predicate(item)` is `true`. If none match, return `default`.

`Func<T, bool>` is a delegate type meaning "a function that takes one `T` and returns a `bool`" — you don't need to understand delegates deeply yet, just know you can call it like a method: `predicate(item)`.

## Rules
- `MyStack<T>` must work identically for `MyStack<int>` and `MyStack<string>` — the same class, no changes needed per type. That's the whole point of generics.
- `Find` needs `where T : ...`? No — it doesn't call anything on `T` itself (only on `predicate`), so no constraint is needed here.
