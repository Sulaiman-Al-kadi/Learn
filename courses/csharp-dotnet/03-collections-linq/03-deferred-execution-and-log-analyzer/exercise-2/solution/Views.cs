public static class Views
{
    public static IEnumerable<int> LiveEvens(List<int> numbers)
    {
        return numbers.Where(n => n % 2 == 0);
    }

    public static List<int> SnapshotEvens(List<int> numbers)
    {
        return numbers.Where(n => n % 2 == 0).ToList();
    }

    public static int CountCalls(List<int> numbers)
    {
        int calls = 0;
        var query = numbers.Where(n => { calls++; return n % 2 == 0; });
        query.Count();
        query.Count();
        return calls;
    }
}
