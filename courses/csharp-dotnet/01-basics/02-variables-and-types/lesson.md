# 02 — Variables & Types

In lesson 01 you printed fixed text. Real programs need to **remember** things — a user's name, a score, a price — and use them later. A **variable** is a named box that holds a value.

## Declaring a variable

```csharp
int age = 25;
```

| Piece | Meaning |
|---|---|
| `int` | The **type** — what *kind* of value the box can hold. `int` = integer = whole number. |
| `age` | The **name** you chose. You'll use it to get the value back. |
| `=` | **Assignment**: "put the thing on the right into the box on the left". It is *not* the math "equals". |
| `25` | The value. |
| `;` | End of statement, as always. |

Now `age` can be used anywhere you'd use the number:

```csharp
int age = 25;
Console.WriteLine(age);        // 25
Console.WriteLine(age + 1);    // 26   (age itself is still 25)
age = 30;                      // change the value — no `int` this time, the box already exists
Console.WriteLine(age);        // 30
```

You declare a variable **once** (with the type). After that you just use its name.

## The types you need right now

C# is **statically typed**: every variable has one type, fixed forever. You can't put text into an `int` box.

| Type | Holds | Example | Notes |
|---|---|---|---|
| `int` | whole numbers | `int count = 42;` | about −2 billion to +2 billion |
| `double` | decimal numbers | `double price = 9.99;` | "double precision floating point" |
| `string` | text | `string name = "Sara";` | always in double quotes |
| `bool` | true or false | `bool isOpen = true;` | only these two values, no quotes |
| `char` | one single character | `char grade = 'A';` | **single** quotes |

```csharp
int    students = 30;
double average  = 87.5;
string course   = "C#";
bool   passed   = true;
char   initial  = 'S';
```

Mixing them up is a compile error — and that's good, it catches bugs early:

```csharp
int x = "hello";     // ERROR: cannot convert string to int
string s = 5;        // ERROR: cannot convert int to string
bool b = "true";     // ERROR: that's a string, not a bool
```

## Putting variables inside text — string interpolation

You'll constantly want to print a sentence with values inside it. The clean way is **interpolation**: put `$` before the opening quote, then wrap any variable or expression in `{ }`:

```csharp
string name = "Sara";
int age = 25;
Console.WriteLine($"{name} is {age} years old.");        // Sara is 25 years old.
Console.WriteLine($"Next year she will be {age + 1}.");  // Next year she will be 26.
```

Without the `$`, the braces are just text: `"{name}"` prints literally `{name}`. Forgetting the `$` is a very common mistake — if you see curly braces in your output, that's why.

The old way (still works, but messier) is joining with `+`:

```csharp
Console.WriteLine(name + " is " + age + " years old.");
```

Use interpolation. It's easier to read.

## `var` — let the compiler figure out the type

```csharp
var name = "Sara";     // compiler sees a string → name is a string
var age = 25;          // → int
var price = 9.99;      // → double
```

`var` does **not** mean "any type". The type is still fixed — the compiler just infers it from the value on the right. `var x = 5; x = "hi";` is still an error. Use `var` when the type is obvious from the right-hand side.

## Naming rules and conventions

Rules (the compiler enforces these):
- Letters, digits, underscore. Must not start with a digit. `age2` ✅ `2age` ❌
- Case-sensitive: `age` and `Age` are different variables.
- Can't be a keyword: `int`, `string`, `if`, `class`... are taken.

Conventions (other programmers expect these):
- **camelCase** for variables: `firstName`, `totalPrice`, `isLoggedIn`.
- Descriptive names. `studentCount` not `sc`. `price` not `p`. Your future self will thank you.
- `bool` names read like a yes/no question: `isOpen`, `hasPaid`, `canEdit`.

## Constants — values that never change

```csharp
const double TaxRate = 0.15;
TaxRate = 0.2;    // ERROR: cannot assign to a const
```

Use `const` for things like tax rates, max sizes, fixed labels. Convention: PascalCase.

## Summary

- `type name = value;` declares a variable. Declare once, then use the name.
- Core types: `int`, `double`, `string`, `bool`, `char`.
- The type is fixed. Wrong type = compile error (a *good* thing).
- `$"..{variable}.."` puts values into text. Don't forget the `$`.
- `var` infers the type from the value; it's still one fixed type.
- camelCase names, descriptive. `const` for values that never change.
