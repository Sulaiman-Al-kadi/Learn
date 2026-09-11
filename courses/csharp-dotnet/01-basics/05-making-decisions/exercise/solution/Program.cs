// Exercise 05 — Cinema ticket pricing (solution)

Console.Write("Age: ");
int age = int.Parse(Console.ReadLine() ?? "");
Console.Write("Student (yes/no): ");
bool isStudent = (Console.ReadLine() ?? "").ToLower() == "yes";
Console.Write("Day: ");
string day = (Console.ReadLine() ?? "").ToLower();
Console.WriteLine();

// 1. base price + category — order matters: first true branch wins
string category;
int price;
if (age < 5)
{
    category = "Child";
    price = 0;
}
else if (age <= 12)
{
    category = "Kid";
    price = 20;
}
else if (age <= 64)
{
    category = "Adult";
    price = 40;
}
else
{
    category = "Senior";
    price = 25;
}

// 2. student discount only applies to the full adult price
if (isStudent && price == 40)
{
    price = 30;
}

// 3. weekend surcharge, but never on a free ticket
bool isWeekend = day == "friday" || day == "saturday";
if (isWeekend && price > 0)
{
    price += 10;
}

Console.WriteLine($"Category: {category}");
Console.WriteLine($"Price: {price}");
Console.WriteLine(price > 0 ? "Enjoy the show!" : "Free entry!");
