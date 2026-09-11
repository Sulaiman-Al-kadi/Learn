// Exercise 07 — Helper method library (solution)

public static class Helpers
{
    public static int Square(int n)
    {
        return n * n;
    }

    public static bool IsEven(int n)
    {
        return n % 2 == 0;
    }

    public static int Max3(int a, int b, int c)
    {
        return Math.Max(a, Math.Max(b, c));
    }

    public static string Repeat(string text, int times)
    {
        if (times <= 0) return "";
        string result = "";
        for (int i = 0; i < times; i++)
        {
            result += text;
        }
        return result;
    }

    public static (int Min, int Max) MinMax(int a, int b, int c)
    {
        int min = Math.Min(a, Math.Min(b, c));
        int max = Math.Max(a, Math.Max(b, c));
        return (min, max);
    }
}
