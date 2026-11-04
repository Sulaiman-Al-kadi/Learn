// Exercise 07b — Aliases and copies (solution)

int n = int.Parse(Console.ReadLine() ?? "");
int[] original = new int[n];
for (int i = 0; i < n; i++)
{
    original[i] = int.Parse(Console.ReadLine() ?? "");
}

int[] alias = original;
int[] copy = (int[])original.Clone();

alias[0] = 100;

Console.WriteLine($"Original: {string.Join(", ", original)}");
Console.WriteLine($"Alias: {string.Join(", ", alias)}");
Console.WriteLine($"Copy: {string.Join(", ", copy)}");
Console.WriteLine($"Alias is original: {object.ReferenceEquals(alias, original)}");
Console.WriteLine($"Copy is original: {object.ReferenceEquals(copy, original)}");
