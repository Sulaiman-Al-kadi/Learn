// Calculator.cs — already implemented and fixed. This is what you're writing tests AGAINST.
// (It's identical in starter/ and solution/ too — you never edit this file.)

public static class Calculator
{
    public static int Add(int a, int b) => a + b;

    public static int Subtract(int a, int b) => a - b;

    public static double Divide(int a, int b)
    {
        if (b == 0) throw new DivideByZeroException("Cannot divide by zero");
        return (double)a / b;
    }

    public static bool IsPrime(int n)
    {
        if (n < 2) return false;
        for (int i = 2; i * i <= n; i++)
        {
            if (n % i == 0) return false;
        }
        return true;
    }
}
