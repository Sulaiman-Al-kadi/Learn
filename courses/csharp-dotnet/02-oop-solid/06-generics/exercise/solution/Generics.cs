// Exercise 06 — Generic stack + Find (solution)

public class MyStack<T>
{
    private List<T> _items = new List<T>();

    public void Push(T item) => _items.Add(item);

    public T Pop()
    {
        T last = _items[_items.Count - 1];
        _items.RemoveAt(_items.Count - 1);
        return last;
    }

    public T Peek() => _items[_items.Count - 1];

    public int Count => _items.Count;
}

public static class GenericTools
{
    public static T? Find<T>(List<T> items, Func<T, bool> predicate)
    {
        foreach (T item in items)
        {
            if (predicate(item)) return item;
        }
        return default;
    }
}
