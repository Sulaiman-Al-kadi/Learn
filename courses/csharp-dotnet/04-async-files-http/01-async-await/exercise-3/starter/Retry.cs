public static class Retry
{
    public static async Task<T> RetryAsync<T>(Func<Task<T>> action, int attempts)
    {
        throw new NotImplementedException();
    }

    public static async Task<T> OrDefaultAsync<T>(Func<Task<T>> action, T fallback)
    {
        throw new NotImplementedException();
    }
}
