using Xunit;

public class ViewsTests
{
    [Fact] public void Live_SeesLaterChanges()
    {
        List<int> numbers = [1, 2];
        var live = Views.LiveEvens(numbers);
        numbers.Add(4);
        Assert.Equal([2, 4], live);
    }
    [Fact] public void Snapshot_IgnoresLaterChanges()
    {
        List<int> numbers = [1, 2];
        var snapshot = Views.SnapshotEvens(numbers);
        numbers.Add(4);
        Assert.Equal([2], snapshot);
    }
    [Fact] public void CountCalls_RunsTwicePerElement() => Assert.Equal(6, Views.CountCalls([1, 2, 3]));
}
