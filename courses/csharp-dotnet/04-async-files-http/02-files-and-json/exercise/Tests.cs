using Xunit;

public class JsonFileStoreTests
{
    private static List<Student> SampleStudents() => new List<Student>
    {
        new Student("Sara", 25, new List<int> { 88, 92 }),
        new Student("Ali", 30, new List<int> { 70, 80, 90 }),
    };

    // Student's Grades property is a List<int>, which does NOT have value equality (only the
    // record's own properties get that for free — a nested List<T> is still compared by reference).
    // So we compare field-by-field, using SequenceEqual for the list, instead of Assert.Equal on
    // the whole Student.
    private static void AssertStudentsEqual(List<Student> expected, List<Student> actual)
    {
        Assert.Equal(expected.Count, actual.Count);
        for (int i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i].Name, actual[i].Name);
            Assert.Equal(expected[i].Age, actual[i].Age);
            Assert.Equal(expected[i].Grades, actual[i].Grades);   // xUnit's Assert.Equal DOES do element-wise comparison for two lists directly
        }
    }

    [Fact]
    public void ToJson_ThenFromJson_RoundTripsEqual()
    {
        var original = SampleStudents();
        string json = JsonFileStore.ToJson(original);
        List<Student> result = JsonFileStore.FromJson(json);

        AssertStudentsEqual(original, result);
    }

    [Fact]
    public void ToJson_ProducesValidJsonContainingNames()
    {
        string json = JsonFileStore.ToJson(SampleStudents());
        Assert.Contains("Sara", json);
        Assert.Contains("Ali", json);
    }

    [Fact]
    public void FromJson_EmptyArray_ReturnsEmptyList()
    {
        var result = JsonFileStore.FromJson("[]");
        Assert.Empty(result);
    }

    [Fact]
    public void SaveToFile_ThenLoadFromFile_RoundTripsEqual()
    {
        string path = Path.Combine(Path.GetTempPath(), $"learnlab-{Guid.NewGuid()}.json");
        try
        {
            var original = SampleStudents();
            JsonFileStore.SaveToFile(original, path);

            Assert.True(File.Exists(path));
            var loaded = JsonFileStore.LoadFromFile(path);

            AssertStudentsEqual(original, loaded);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public void LoadFromFile_MissingFile_ReturnsEmptyListWithoutThrowing()
    {
        string path = Path.Combine(Path.GetTempPath(), $"learnlab-does-not-exist-{Guid.NewGuid()}.json");
        var result = JsonFileStore.LoadFromFile(path);
        Assert.Empty(result);
    }
}
