public static class Fetcher
{
    public static async Task<int> SumAllAsync(List<int> ids, Func<int, Task<int>> fetch)
    {
        throw new NotImplementedException();
    }

    public static async Task<List<int>> FetchInOrderAsync(List<int> ids, Func<int, Task<int>> fetch)
    {
        throw new NotImplementedException();
    }
}
