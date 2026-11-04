using Xunit;

public class TextCleanupTests
{
    [Fact] public void CleanLines_TrimsAndDrops() => Assert.Equal(["a", "b c"], TextCleanup.CleanLines(["  a ", "   ", "", "b c  "]));
    [Fact] public void WordLengths_Basic() => Assert.Equal([2, 5], TextCleanup.WordLengths("hi there"));
    [Fact] public void WordLengths_ExtraSpaces() => Assert.Equal([1, 1], TextCleanup.WordLengths("a   b"));
    [Fact] public void LongestFirst_BreaksTies() => Assert.Equal(["sara", "zaid", "ali"], TextCleanup.LongestFirst(["ali", "zaid", "sara"]));
}
