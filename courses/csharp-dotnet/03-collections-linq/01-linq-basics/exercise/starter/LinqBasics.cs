// Exercise 01 — LINQ basics toolkit
// See README.md. Every method should be a single LINQ chain ending in .ToList() — no foreach.

public record Student(string Name, int Score);

public static class LinqTools
{
    public static List<int> Evens(List<int> numbers)
    {
        throw new NotImplementedException();
    }

    public static List<string> UpperNames(List<string> names)
    {
        throw new NotImplementedException();
    }

    public static List<string> LongNamesUppercased(List<string> names, int minLength)
    {
        throw new NotImplementedException();
    }

    public static List<int> SortedDescending(List<int> numbers)
    {
        throw new NotImplementedException();
    }

    public static List<string> PassingStudentNames(List<Student> students, int passingScore)
    {
        throw new NotImplementedException();
    }
}
