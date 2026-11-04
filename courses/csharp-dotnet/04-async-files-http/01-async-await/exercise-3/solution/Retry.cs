public static class Retry
{
    public static async Task<T> RetryAsync<T>(Func<Task<T>> action, int attempts)
    {
        for (int i = 1; ; i++)
        {
            try { return await action(); }
            catch (Exception) when (i < attempts) { }
        }
    }

    public static async Task<T> OrDefaultAsync<T>(Func<Task<T>> action, T fallback)
    {
        try { return await action(); }
        catch (Exception) { return fallback; }
    }
}
