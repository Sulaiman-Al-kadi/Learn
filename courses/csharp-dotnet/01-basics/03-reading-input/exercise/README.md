# Exercise — Order calculator

Write a program that asks four questions, reads the answers, and prints a summary.

The checker will "type" these four lines for you (they're in `input.txt`):

```
Sara
Notebook
12.5
4
```

Your program must print **exactly**:

```
Customer name: Item: Price: Quantity: 
Order for Sara
4 x Notebook at 12.5 each
Total: 50
```

Why is the first line so odd? Because the checker feeds input silently — the typed answers don't echo to the screen. So all four prompts (`Customer name: `, `Item: `, `Price: `, `Quantity: `) end up on one line, each ending with a space, and then a blank `WriteLine()` ends that line. When *you* run it in a terminal it will look normal.

## Rules
1. Prompt with `Console.Write` for each question, in this order: `Customer name: `, `Item: `, `Price: `, `Quantity: ` (note the trailing space after each colon).
2. Read each answer with `Console.ReadLine() ?? ""`.
3. Convert price with `double.Parse` and quantity with `int.Parse`.
4. After the four prompts, call `Console.WriteLine();` once to end the prompt line.
5. `Total` must be computed (`price * quantity`), not typed.

Run it yourself too: in the terminal, `dotnet run Program.cs` from this folder, and type answers by hand.
