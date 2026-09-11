using Xunit;

public class LinqBasicsTests
{
    [Fact]
    public void Evens_FiltersCorrectly()
    {
        var result = LinqTools.Evens(new List<int> { 1, 2, 3, 4, 5, 6 });
        Assert.Equal(new List<int> { 2, 4, 6 }, result);
    }

    [Fact]
    public void Evens_NoneMatch_ReturnsEmpty()
    {
        var result = LinqTools.Evens(new List<int> { 1, 3, 5 });
        Assert.Empty(result);
    }

    [Fact]
    public void UpperNames_UppercasesAll()
    {
        var result = LinqTools.UpperNames(new List<string> { "ali", "sara" });
        Assert.Equal(new List<string> { "ALI", "SARA" }, result);
    }

    [Fact]
    public void LongNamesUppercased_FiltersThenTransforms()
    {
        var result = LinqTools.LongNamesUppercased(new List<string> { "ali", "sara", "om", "khalid" }, 3);
        Assert.Equal(new List<string> { "SARA", "KHALID" }, result);
    }

    [Fact]
    public void SortedDescending_SortsCorrectly()
    {
        var result = LinqTools.SortedDescending(new List<int> { 5, 2, 8, 1, 9 });
        Assert.Equal(new List<int> { 9, 8, 5, 2, 1 }, result);
    }

    [Fact]
    public void PassingStudentNames_FiltersOrdersAndProjects()
    {
        var students = new List<Student>
        {
            new Student("Ali", 85),
            new Student("Sara", 92),
            new Student("Omar", 67),
            new Student("Lina", 70),
        };

        var result = LinqTools.PassingStudentNames(students, 70);

        Assert.Equal(new List<string> { "Sara", "Ali", "Lina" }, result);
    }

    [Fact]
    public void PassingStudentNames_NoneQualify_ReturnsEmpty()
    {
        var students = new List<Student> { new Student("Omar", 40) };
        var result = LinqTools.PassingStudentNames(students, 70);
        Assert.Empty(result);
    }
}
