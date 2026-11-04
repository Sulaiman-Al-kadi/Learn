using Xunit;

public class ExamStatsTests
{
    private static readonly List<Exam> Exams =
    [
        new("Sara", "Math", 90), new("Ali", "Math", 70), new("Omar", "Art", 50), new("Lina", "Art", 60),
    ];

    [Fact] public void Average_PerSubject()
    {
        var result = ExamStats.AverageBySubject(Exams);
        Assert.Equal(80, result["Math"]);
        Assert.Equal(55, result["Art"]);
    }
    [Fact] public void TopScorer_Finds() => Assert.Equal("Sara", ExamStats.TopScorer(Exams));
    [Fact] public void TopScorer_Empty() => Assert.Null(ExamStats.TopScorer([]));
    [Theory] [InlineData(50, true)] [InlineData(51, false)]
    public void EveryonePassed_UsesMark(int mark, bool expected) => Assert.Equal(expected, ExamStats.EveryonePassed(Exams, mark));
    [Fact] public void NamesPage_SecondPage() => Assert.Equal(["Omar", "Sara"], ExamStats.NamesPage(Exams, 2, 2));
    [Fact] public void NamesPage_PastEnd() => Assert.Empty(ExamStats.NamesPage(Exams, 3, 2));
}
