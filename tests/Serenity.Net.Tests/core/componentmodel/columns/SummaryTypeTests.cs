namespace Serenity;

public class SummaryTypeTests
{
    [Fact]
    public void SummaryType_Values_AreCorrect()
    {
        Assert.Equal(-1, (int)SummaryType.Disabled);
        Assert.Equal(0, (int)SummaryType.None);
        Assert.Equal(1, (int)SummaryType.Sum);
        Assert.Equal(2, (int)SummaryType.Avg);
        Assert.Equal(3, (int)SummaryType.Min);
        Assert.Equal(4, (int)SummaryType.Max);
    }
}