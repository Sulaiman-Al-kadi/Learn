using Xunit;

public class GenericsTests
{
    [Fact]
    public void MyStack_Int_IsLifo()
    {
        var s = new MyStack<int>();
        s.Push(1);
        s.Push(2);
        s.Push(3);
        Assert.Equal(3, s.Pop());
        Assert.Equal(2, s.Pop());
        Assert.Equal(1, s.Pop());
    }

    [Fact]
    public void MyStack_String_WorksTheSameWay()
    {
        var s = new MyStack<string>();
        s.Push("a");
        s.Push("b");
        Assert.Equal("b", s.Pop());
    }

    [Fact]
    public void MyStack_Peek_DoesNotRemove()
    {
        var s = new MyStack<int>();
        s.Push(10);
        s.Push(20);
        Assert.Equal(20, s.Peek());
        Assert.Equal(2, s.Count);
        Assert.Equal(20, s.Pop());
        Assert.Equal(1, s.Count);
    }

    [Fact]
    public void MyStack_Count_TracksPushesAndPops()
    {
        var s = new MyStack<int>();
        Assert.Equal(0, s.Count);
        s.Push(1);
        s.Push(2);
        Assert.Equal(2, s.Count);
        s.Pop();
        Assert.Equal(1, s.Count);
    }

    [Fact]
    public void Find_ReturnsFirstMatch()
    {
        var numbers = new List<int> { 1, 4, 6, 9, 12 };
        int result = GenericTools.Find(numbers, n => n % 3 == 0);
        Assert.Equal(6, result);
    }

    [Fact]
    public void Find_NoMatch_ReturnsDefaultForInt()
    {
        var numbers = new List<int> { 1, 2, 4 };
        int result = GenericTools.Find(numbers, n => n > 100);
        Assert.Equal(0, result);
    }

    [Fact]
    public void Find_NoMatch_ReturnsNullForString()
    {
        var words = new List<string> { "cat", "dog" };
        string? result = GenericTools.Find(words, w => w.StartsWith("z"));
        Assert.Null(result);
    }

    [Fact]
    public void Find_WorksOnStrings()
    {
        var words = new List<string> { "cat", "dog", "zebra" };
        string? result = GenericTools.Find(words, w => w.Length > 3);
        Assert.Equal("zebra", result);
    }
}
