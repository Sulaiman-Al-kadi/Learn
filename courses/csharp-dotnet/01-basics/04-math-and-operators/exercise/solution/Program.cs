// Exercise 04 — Grade statistics (solution)

Console.Write("Score 1: ");
int s1 = int.Parse(Console.ReadLine() ?? "");
Console.Write("Score 2: ");
int s2 = int.Parse(Console.ReadLine() ?? "");
Console.Write("Score 3: ");
int s3 = int.Parse(Console.ReadLine() ?? "");
Console.WriteLine();

int sum = s1 + s2 + s3;
double average = Math.Round((double)sum / 3, 2);   // cast first, or int/int drops the decimals
int highest = Math.Max(s1, Math.Max(s2, s3));
int lowest = Math.Min(s1, Math.Min(s2, s3));

Console.WriteLine($"Sum: {sum}");
Console.WriteLine($"Average: {average}");
Console.WriteLine($"Highest: {highest}");
Console.WriteLine($"Lowest: {lowest}");
Console.WriteLine($"Range: {highest - lowest}");
Console.WriteLine($"Sum is even: {sum % 2 == 0}");
Console.WriteLine($"Last digit of sum: {sum % 10}");
