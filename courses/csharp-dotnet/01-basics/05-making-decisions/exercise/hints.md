## Hint 1
Turn the text into a bool once, up front: `bool isStudent = studentAnswer.ToLower() == "yes";` and `day = day.ToLower();` — then every later comparison is simple.

## Hint 2
Declare `string category;` and `int price;` *before* the chain, then assign both inside each branch. Since every branch assigns them, C# knows they're set afterwards.

## Hint 3
Order the age checks from smallest up: `if (age < 5) ... else if (age <= 12) ... else if (age <= 64) ... else ...`. Because earlier branches already caught the smaller ages, each later branch only needs an upper bound.

## Hint 4
Weekend: `bool isWeekend = day == "friday" || day == "saturday";` then `if (isWeekend && price > 0) { price += 10; }`. The closing message is a one-liner with `? :`.
