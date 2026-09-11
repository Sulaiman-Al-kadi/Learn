// Exercise 08 — List toolkit (solution)

int n = int.Parse(Console.ReadLine() ?? "");
List<int> original = new List<int>();
for (int i = 0; i < n; i++)
{
    original.Add(int.Parse(Console.ReadLine() ?? ""));
}

List<int> sorted = new List<int>(original);
sorted.Sort();

List<int> reversed = new List<int>(original);
reversed.Reverse();

List<int> unique = new List<int>();
foreach (int value in original)
{
    if (!unique.Contains(value))
    {
        unique.Add(value);
    }
}

int largest = int.MinValue;
int second = int.MinValue;
bool hasSecond = false;
foreach (int value in unique)
{
    if (value > largest)
    {
        second = largest;
        hasSecond = largest != int.MinValue;
        largest = value;
    }
    else if (value > second || !hasSecond)
    {
        second = value;
        hasSecond = true;
    }
}

Console.WriteLine($"Original: {string.Join(", ", original)}");
Console.WriteLine($"Sorted: {string.Join(", ", sorted)}");
Console.WriteLine($"Reversed: {string.Join(", ", reversed)}");
Console.WriteLine($"Unique: {string.Join(", ", unique)}");
Console.WriteLine(unique.Count >= 2 ? $"Second largest: {second}" : "Second largest: none");
