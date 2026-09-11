# Exercise — Print a receipt

Make `Program.cs` print **exactly** this (every character matters, including the tab and the quotes):

```
=== Coffee Shop ===
Item:	Latte
Price:	15
Qty:	2
Total:	30
Thanks for visiting "Bean & Co"!
Path: C:\shop\receipts
```

## Rules
1. The gap after `Item:` / `Price:` / `Qty:` / `Total:` is a **tab** — use `\t`, not spaces.
2. `Total` must be **calculated** by C# from `15 * 2`. Don't type `30`.
3. Use `Console.Write` at least once (for example, to print `Total:` and the number separately).
4. The quotes around `Bean & Co` are part of the output.
5. The path line contains real backslashes.

## How to check
Save the file, then press the **beaker icon** in the Learn Lab sidebar, or `Ctrl+Alt+Enter`.
If it fails, choose **Show diff** to see exactly which line is wrong.

Stuck? Press the **lightbulb** for a hint.
