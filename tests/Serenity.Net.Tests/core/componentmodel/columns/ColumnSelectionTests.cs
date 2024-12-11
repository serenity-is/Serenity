namespace Serenity.Services;

public class ColumnSelectionTests()
{
    [Fact]
    public void ColumnSelection_Values_AreCorrect()
    {
        Assert.Equal(0, (int)ColumnSelection.List);
        Assert.Equal(1, (int)ColumnSelection.KeyOnly);
        Assert.Equal(2, (int)ColumnSelection.Details);
        Assert.Equal(3, (int)ColumnSelection.None);
        Assert.Equal(4, (int)ColumnSelection.IdOnly);
        Assert.Equal(5, (int)ColumnSelection.Lookup);
    }
}