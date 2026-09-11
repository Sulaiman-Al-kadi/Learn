## Hint 1
Fix the given test first: `Assert.Equal(5, Calculator.Add(2, 3));` — that's your template for every other simple case too.

## Hint 2
`Divide` by zero:
```csharp
[Fact]
public void Divide_ByZero_ThrowsDivideByZeroException()
{
    Assert.Throws<DivideByZeroException>(() => Calculator.Divide(10, 0));
}
```

## Hint 3
`IsPrime` with a `[Theory]` avoids writing five near-identical `[Fact]`s:
```csharp
[Theory]
[InlineData(7, true)]
[InlineData(8, false)]
[InlineData(1, false)]
[InlineData(2, true)]
[InlineData(0, false)]
public void IsPrime_Works(int number, bool expected)
{
    Assert.Equal(expected, Calculator.IsPrime(number));
}
```
