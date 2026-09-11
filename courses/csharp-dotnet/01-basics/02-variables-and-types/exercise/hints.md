## Hint 1
Declaring looks like `type name = value;`. Five of them, one per line:
```csharp
string name = "Layla Hassan";
int age = 21;
// ... and so on for double, bool, char
```
Remember: `char` uses single quotes `'L'`, `bool` has no quotes at all.

## Hint 2
Interpolation puts a variable inside text: `Console.WriteLine($"Name:      {name}");` — don't forget the `$` before the quote.

## Hint 3
Math works inside the braces: `{age + 4}`. The whole last line is one `WriteLine` with two placeholders: `$"In 4 years {name} will be {age + 4}."`
