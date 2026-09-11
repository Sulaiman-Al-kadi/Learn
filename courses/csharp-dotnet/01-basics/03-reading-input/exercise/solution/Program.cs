// Exercise 03 — Order calculator (solution)

Console.Write("Customer name: ");
string customer = Console.ReadLine() ?? "";

Console.Write("Item: ");
string item = Console.ReadLine() ?? "";

Console.Write("Price: ");
double price = double.Parse(Console.ReadLine() ?? "");

Console.Write("Quantity: ");
int quantity = int.Parse(Console.ReadLine() ?? "");

Console.WriteLine();
Console.WriteLine($"Order for {customer}");
Console.WriteLine($"{quantity} x {item} at {price} each");
Console.WriteLine($"Total: {price * quantity}");
