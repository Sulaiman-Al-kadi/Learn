## Hint 1
`Evens`: `return numbers.Where(n => n % 2 == 0).ToList();`

## Hint 2
`LongNamesUppercased` chains two steps — filter first, then transform: `names.Where(n => n.Length > minLength).Select(n => n.ToUpper()).ToList();`

## Hint 3
`SortedDescending`: `numbers.OrderByDescending(n => n).ToList();` — the sort key IS the number itself.

## Hint 4
`PassingStudentNames` chains three: `students.Where(s => s.Score >= passingScore).OrderByDescending(s => s.Score).Select(s => s.Name).ToList();`
