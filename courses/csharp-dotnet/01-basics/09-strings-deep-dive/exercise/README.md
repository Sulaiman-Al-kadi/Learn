# Exercise — Text tools library

Same format as lesson 07's exercise: implement the methods in `Methods.cs`, checked by the (read-only) `Tests.cs`. This time every method is about text.

| Method | Does |
|---|---|
| `string NormalizeName(string name)` | Trim, collapse extra internal whitespace, and title-case each word: first letter upper, rest lower. `"  aLI   hassan "` → `"Ali Hassan"` |
| `string Initials(string fullName)` | Uppercase first letter of each word, concatenated, no spaces. `"ali hassan"` → `"AH"`. Empty input → `""` |
| `string MaskEmail(string email)` | First character, then `***`, then everything from `@` onward. `"ali@example.com"` → `"a***@example.com"`. If there's no `@` at all, return the input unchanged |
| `int SafeParseInt(string text, int fallback)` | Parse `text` as an int; if it's not a valid number, return `fallback` instead of crashing |
| `int CountWords(string text)` | Number of whitespace-separated words, ignoring extra/leading/trailing spaces. `"  the  quick fox "` → `3` |

## Rules
- `NormalizeName` and `CountWords`: split on whitespace with `StringSplitOptions.RemoveEmptyEntries` — that handles "extra spaces" for you.
- Title-casing one word: `char.ToUpper(word[0]) + word.Substring(1).ToLower()` — but only for non-empty words.
- `Initials`: same split, take `word[0]` of each (uppercased), no separator when joining.
- `MaskEmail`: find `@` with `IndexOf`; if it's `-1`, there's no `@` — return the input as-is.
- `SafeParseInt`: this is exactly what `TryParse` is for — no `try`/`catch` needed.

Check with the beaker. Read `Tests.cs` afterward to see every case that was checked.
