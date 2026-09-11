// Exercise 06 — Generic stack + Find
// See README.md.

public class MyStack<T>
{
    // TODO: store items (a List<T> works well), and implement Push/Pop/Peek/Count
    public void Push(T item)
    {
        throw new NotImplementedException();
    }

    public T Pop()
    {
        throw new NotImplementedException();
    }

    public T Peek()
    {
        throw new NotImplementedException();
    }

    public int Count => throw new NotImplementedException();
}

public static class GenericTools
{
    // TODO: walk `items`, return the first one where predicate(item) is true, else default
    public static T? Find<T>(List<T> items, Func<T, bool> predicate)
    {
        throw new NotImplementedException();
    }
}
