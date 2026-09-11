// Exercise 01 — Async toolkit (solution)

public static class AsyncTools
{
    public static async Task<int> SquareAfterDelayAsync(int n, int delayMs)
    {
        await Task.Delay(delayMs);
        return n * n;
    }

    public static async Task<List<int>> SquareAllAsync(List<int> numbers, int delayMs)
    {
        List<Task<int>> tasks = numbers.Select(n => SquareAfterDelayAsync(n, delayMs)).ToList();
        int[] results = await Task.WhenAll(tasks);
        return results.ToList();
    }

    public static async Task<int> SafeDivideAsync(int a, int b)
    {
        await Task.Delay(10);
        if (b == 0) throw new DivideByZeroException();
        return a / b;
    }
}
