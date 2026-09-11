// Tests.cs — the oracle. Don't edit this.
using Xunit;

public class TextToolsTests
{
    [Fact] public void NormalizeName_MixedCaseAndSpacing() => Assert.Equal("Ali Hassan", TextTools.NormalizeName("  aLI   hassan "));
    [Fact] public void NormalizeName_AllCaps() => Assert.Equal("Omar", TextTools.NormalizeName("OMAR"));
    [Fact] public void NormalizeName_AlreadyClean() => Assert.Equal("Sara Ahmed", TextTools.NormalizeName("Sara Ahmed"));
    [Fact] public void NormalizeName_Empty() => Assert.Equal("", TextTools.NormalizeName(""));

    [Fact] public void Initials_TwoWords() => Assert.Equal("AH", TextTools.Initials("ali hassan"));
    [Fact] public void Initials_OneWord() => Assert.Equal("S", TextTools.Initials("Sara"));
    [Fact] public void Initials_Empty() => Assert.Equal("", TextTools.Initials(""));
    [Fact] public void Initials_ThreeWords() => Assert.Equal("OBK", TextTools.Initials("omar bin khalid"));

    [Fact] public void MaskEmail_Basic() => Assert.Equal("a***@example.com", TextTools.MaskEmail("ali@example.com"));
    [Fact] public void MaskEmail_ShortLocalPart() => Assert.Equal("b***@test.io", TextTools.MaskEmail("bob@test.io"));
    [Fact] public void MaskEmail_NoAtSign() => Assert.Equal("noatsign", TextTools.MaskEmail("noatsign"));

    [Fact] public void SafeParseInt_Valid() => Assert.Equal(42, TextTools.SafeParseInt("42", -1));
    [Fact] public void SafeParseInt_Invalid() => Assert.Equal(-1, TextTools.SafeParseInt("abc", -1));
    [Fact] public void SafeParseInt_EmptyUsesFallback() => Assert.Equal(0, TextTools.SafeParseInt("", 0));

    [Fact] public void CountWords_Normal() => Assert.Equal(4, TextTools.CountWords("the quick brown fox"));
    [Fact] public void CountWords_ExtraSpaces() => Assert.Equal(3, TextTools.CountWords("  the  quick fox "));
    [Fact] public void CountWords_Empty() => Assert.Equal(0, TextTools.CountWords(""));
    [Fact] public void CountWords_OnlySpaces() => Assert.Equal(0, TextTools.CountWords("   "));
    [Fact] public void CountWords_OneWord() => Assert.Equal(1, TextTools.CountWords("hello"));
}
