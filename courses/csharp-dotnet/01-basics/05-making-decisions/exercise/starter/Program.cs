// Exercise 05 — Cinema ticket pricing
// Read README.md carefully — the rules must be applied in order. Check the 4 cases in cases/.

Console.Write("Age: ");
int age = int.Parse(Console.ReadLine() ?? "");
Console.Write("Student (yes/no): ");
string studentAnswer = Console.ReadLine() ?? "";
Console.Write("Day: ");
string day = Console.ReadLine() ?? "";
Console.WriteLine();

// TODO: turn studentAnswer into a bool (any casing of "yes" counts). Lower-case `day` too.

// TODO: 1. base price and category with an if / else if / else chain

// TODO: 2. student discount

// TODO: 3. weekend surcharge (friday or saturday), only if price > 0

// TODO: print Category, Price, and the closing message
