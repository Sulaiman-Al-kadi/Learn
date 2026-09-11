# Exercise — Cinema ticket pricing

A cinema prices tickets by age, with a student discount and a weekend surcharge. Write the program that asks three questions and prints the price.

## Input (three lines)
1. Age — a whole number
2. Student? — `yes` or `no` (accept any casing: `Yes`, `YES`…)
3. Day — the day name, e.g. `friday` (any casing)

## Pricing rules (apply in this order)
1. **Base price** by age:
   - under 5 → `0` (free)
   - 5 to 12 → `20`
   - 13 to 64 → `40`
   - 65 and over → `25`
2. **Student discount**: if student and base price is `40`, price becomes `30`. (Students don't get a discount on other prices.)
3. **Weekend surcharge**: if the day is `friday` or `saturday`, add `10` — *unless* the ticket is free.

## Output
Prompts: `Age: `, `Student (yes/no): `, `Day: ` — then `Console.WriteLine();` once. Then:

```
Category: <Child|Kid|Adult|Senior>
Price: <number>
```
followed by exactly one of:
- `Enjoy the show!` if price is greater than 0
- `Free entry!` if price is 0

Categories: under 5 = `Child`, 5–12 = `Kid`, 13–64 = `Adult`, 65+ = `Senior`.

## The checker runs 4 cases
The `cases/` folder has `1.in`…`4.in` with matching `.out` files. Open them to see each scenario. **All four must pass.** The output panel tells you which case failed and on which line.

Tip: work out the expected price by hand for each case before coding — that's what a programmer does.
