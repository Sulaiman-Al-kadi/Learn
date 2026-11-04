public static class Fetcher
{
    public static async Task<int> SumAllAsync(List<int> ids, Func<int, Task<int>> fetch)
    {
        int[] results = await Task.WhenAll(ids.Select(fetch));
        return results.Sum();
    }

    public static async Task<List<int>> FetchInOrderAsync(List<int> ids, Func<int, Task<int>> fetch)
    {
        int[] results = await Task.WhenAll(ids.Select(fetch));
        return results.ToList();
    }
}
