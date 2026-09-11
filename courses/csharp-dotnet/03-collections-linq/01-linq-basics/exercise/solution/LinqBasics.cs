// Exercise 01 — LINQ basics toolkit (solution)

public record Student(string Name, int Score);

public static class LinqTools
{
    public static List<int> Evens(List<int> numbers)
    {
        return numbers.Where(n => n % 2 == 0).ToList();
    }

    public static List<string> UpperNames(List<string> names)
    {
        return names.Select(n => n.ToUpper()).ToList();
    }

    public static List<string> LongNamesUppercased(List<string> names, int minLength)
    {
        return names.Where(n => n.Length > minLength).Select(n => n.ToUpper()).ToList();
    }

    public static List<int> SortedDescending(List<int> numbers)
    {
        return numbers.OrderByDescending(n => n).ToList();
    }

    public static List<string> PassingStudentNames(List<Student> students, int passingScore)
    {
        return students
            .Where(s => s.Score >= passingScore)
            .OrderByDescending(s => s.Score)
            .Select(s => s.Name)
            .ToList();
    }
}
