## Hint 1
A `List<T> _items` field is the easiest backing store. "Top of the stack" = the END of the list (last element), because that's O(1) to add/remove — `_items.Add(item)` and `_items.RemoveAt(_items.Count - 1)`.

## Hint 2
```csharp
public T Pop()
{
    T last = _items[_items.Count - 1];
    _items.RemoveAt(_items.Count - 1);
    return last;
}
```
`Peek` is the first line of that, without the removal. `Count` is just `_items.Count` as a computed property (lesson 02): `public int Count => _items.Count;`

## Hint 3
`Find` is a `foreach` with an early return, same pattern as any "search" loop:
```csharp
foreach (T item in items)
{
    if (predicate(item)) return item;
}
return default;
```
