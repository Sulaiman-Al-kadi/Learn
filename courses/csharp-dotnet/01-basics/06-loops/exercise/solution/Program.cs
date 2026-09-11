// Exercise 06 — Number stats + multiplication table (solution)

int count = 0, sum = 0, max = int.MinValue, min = int.MaxValue, evens = 0;

while (true)
{
    int n = int.Parse(Console.ReadLine() ?? "");
    if (n == 0) break;

    count++;
    sum += n;
    max = Math.Max(max, n);
    min = Math.Min(min, n);
    if (n % 2 == 0) evens++;
}

int tableSize = int.Parse(Console.ReadLine() ?? "");

Console.WriteLine($"Numbers: {count}");
if (count > 0)
{
    double average = Math.Round((double)sum / count, 2);
    Console.WriteLine($"Sum: {sum}");
    Console.WriteLine($"Average: {average}");
    Console.WriteLine($"Max: {max}");
    Console.WriteLine($"Min: {min}");
    Console.WriteLine($"Evens: {evens}");
}

Console.WriteLine("Table:");
for (int row = 1; row <= tableSize; row++)
{
    for (int col = 1; col <= tableSize; col++)
    {
        Console.Write($"{row * col,4}");
    }
    Console.WriteLine();
}
