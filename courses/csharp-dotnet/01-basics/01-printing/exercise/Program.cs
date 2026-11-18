// Exercise 01 — Print a receipt
// Read README.md (it opens next to this file). Make the output match EXACTLY.

Console.WriteLine("=== Coffee Shop ===");

// TODO: print the Item line (tab between "Item:" and "Latte")

Console.WriteLine("Item:\tLatte");
// TODO: print the Price and Qty lines
Console.WriteLine("Price:\t15");
Console.WriteLine("Qty:\t2");
// TODO: print "Total:" with Write, then the calculated total with WriteLine
Console.Write("Total:\t");
Console.WriteLine(15*2);
// TODO: print the thank-you line — the quotes are part of the output
Console.WriteLine("Thanks for visiting \"Bean & Co\"!");
// TODO: print the path line
Console.WriteLine("Path:"+" "+ @"C:\shop\receipts" );