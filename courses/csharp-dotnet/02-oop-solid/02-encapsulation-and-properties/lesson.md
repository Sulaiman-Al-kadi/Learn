# Module 2, Lesson 02 — Encapsulation & Properties

Lesson 01's `Car.Mileage` was a public field — anyone could write `car.Mileage = -500;` and there's nothing to stop them. **Encapsulation** means hiding an object's internal data and only allowing controlled access to it. In C#, the tool for this is the **property**.

## The problem with public fields

```csharp
public class BankAccount
{
    public double Balance;
}

var acc = new BankAccount();
acc.Balance = -1000000;    // perfectly legal, silently wrong
```

Nothing enforces "a balance shouldn't be set directly, or shouldn't go negative." Fields have no way to run logic when they change.

## Properties — a field with rules

```csharp
public class BankAccount
{
    private double _balance;                 // the REAL data, hidden (private)

    public double Balance                    // the public "door" to it
    {
        get { return _balance; }
        set
        {
            if (value < 0) throw new ArgumentException("Balance cannot be negative");
            _balance = value;
        }
    }
}
```

| Piece | Meaning |
|---|---|
| `private double _balance;` | The actual storage. `private` means only code **inside this class** can touch it. Convention: leading underscore for a private backing field. |
| `get { return _balance; }` | Runs when someone **reads** `acc.Balance`. |
| `set { ... }` | Runs when someone **writes** `acc.Balance = x;`. `value` is a special name for whatever was assigned. |

From the outside, it still looks like a field: `acc.Balance = 500;` and `Console.WriteLine(acc.Balance);` both work exactly as before — but now every write goes through your validation.

## Auto-properties — when you don't need custom logic

Writing a private field + get + set for every simple property is a lot of ceremony. When there's no extra logic, use an **auto-property**:

```csharp
public class Dog
{
    public string Name { get; set; }
    public int Age { get; set; }
}
```

The compiler generates the hidden backing field for you. This is what you should write **by default** — only drop down to a full property (with an explicit backing field) when you actually need validation or computed logic, like `Balance` above.

## `private set` — read from anywhere, write only inside the class

Very common pattern: the outside world can see a value but never set it directly — only the class's own methods change it:

```csharp
public class BankAccount
{
    public double Balance { get; private set; }        // anyone can READ it; only THIS class can set it

    public BankAccount(double startingBalance)
    {
        Balance = startingBalance;                       // allowed: we're inside the class
    }

    public void Deposit(double amount)
    {
        Balance += amount;                                // allowed: inside the class
    }
}
```

```csharp
var acc = new BankAccount(100);
Console.WriteLine(acc.Balance);   // fine — reading is public
acc.Balance = 999;                // ERROR: the setter is private, can't be called from outside
```

This is usually **better** than a fully public settable property — it forces changes to go through methods like `Deposit`/`Withdraw` that can enforce rules, instead of letting anyone set the value directly.

## Computed (read-only) properties

A property doesn't have to store anything — it can calculate its value every time it's read:

```csharp
public class Rectangle
{
    public double Width { get; set; }
    public double Height { get; set; }

    public double Area => Width * Height;          // expression-bodied property — no `set` at all
}
```

```csharp
var r = new Rectangle { Width = 4, Height = 5 };
Console.WriteLine(r.Area);   // 20 — recalculated every time you read it
r.Width = 10;
Console.WriteLine(r.Area);   // 50 — automatically up to date, nothing to keep in sync manually
```

(That `new Rectangle { Width = 4, Height = 5 }` syntax is an **object initializer** — a shortcut for setting properties right after construction, instead of a separate line for each.)

## `init` — settable only at construction time

For a value that should be fixed forever after an object is created (but you still want the convenience of an object initializer), use `init` instead of `set`:

```csharp
public class Order
{
    public string Id { get; init; } = "";
}

var order = new Order { Id = "ORD-001" };   // fine — this happens during construction
order.Id = "ORD-002";                        // ERROR — can't change it after that
```

## Access modifiers — the vocabulary

| Modifier | Who can access it |
|---|---|
| `public` | anyone |
| `private` | only code inside this same class |
| `protected` | this class, and classes that inherit from it (next lesson) |
| `internal` | anywhere in the same project |

**Rule of thumb**: make fields `private`, expose what's needed through `public` properties and methods. This is encapsulation — the class controls its own data.

## Validating in a full property

Putting it all together — the pattern you'll use whenever a value needs a rule:

```csharp
public class Product
{
    private double _price;

    public string Name { get; set; } = "";

    public double Price
    {
        get => _price;
        set
        {
            if (value < 0) throw new ArgumentException("Price cannot be negative");
            _price = value;
        }
    }
}
```
(`get => _price;` is the expression-bodied shorthand for `get { return _price; }` — same idea as methods in lesson 07.)

## Summary
- **Fields**: raw storage, no rules. Make them `private`.
- **Properties**: the public interface to that data — `get`/`set` can contain logic.
- **Auto-property** `{ get; set; }`: the default when there's no extra logic needed.
- `{ get; private set; }`: readable everywhere, only settable from inside the class — forces changes through methods.
- **Computed property** `=> expression`: no storage, recalculated on every read.
- `init`: settable only during construction (works with object initializers), then locked.
- **Encapsulation** = hide the data (`private` fields), expose controlled access (`public` properties/methods).
