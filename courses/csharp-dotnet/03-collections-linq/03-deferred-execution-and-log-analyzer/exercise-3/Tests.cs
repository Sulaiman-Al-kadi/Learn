using Xunit;

public class WordStatsTests
{
    private const string Text = "the cat and The dog and THE bird";

    [Fact] public void TopWords_CountsIgnoringCase() => Assert.Equal(("the", 3), WordStats.TopWords(Text, 1)[0]);
    [Fact] public void TopWords_BreaksTiesAlphabetically() => Assert.Equal([("the", 3), ("and", 2), ("bird", 1)], WordStats.TopWords(Text, 3));
    [Fact] public void UniqueWords_IgnoresCase() => Assert.Equal(5, WordStats.UniqueWords(Text));
    [Fact] public void UniqueWords_Empty() => Assert.Equal(0, WordStats.UniqueWords(""));
}
