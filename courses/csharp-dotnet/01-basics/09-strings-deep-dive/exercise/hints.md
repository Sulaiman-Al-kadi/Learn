## Hint 1
`name.Split(' ', StringSplitOptions.RemoveEmptyEntries)` turns messy spacing into a clean array of words — this one call handles leading, trailing, AND repeated spaces at once.

## Hint 2
Title-casing one word: `char.ToUpper(word[0]) + word.Substring(1).ToLower()`. `word[0]` is a `char`; `word.Substring(1)` is everything after it.

## Hint 3
`MaskEmail`: `email.IndexOf('@')` gives the position of `@`, or `-1` if it's missing — handle that case first with an early return. Otherwise: `email.Substring(0, 1)` is the first character, `email.Substring(at)` is everything from `@` to the end.

## Hint 4
`SafeParseInt` is a one-liner around `TryParse`:
```csharp
return int.TryParse(text, out int result) ? result : fallback;
```

## Hint 5
`CountWords` — split the same way as Hint 1, then the answer is just `.Length` of the resulting array.
