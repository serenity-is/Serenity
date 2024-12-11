namespace Serenity.ComponentModel;

public class SortOrderAttributeTests
{
    [Fact]
    public void SortOrder_ShouldBeNegative_WhenDescendingIsTrue()
    {
        var attribute = new SortOrderAttribute(1, true);
        Assert.Equal(-1, attribute.SortOrder);
    }

    [Fact]
    public void SortOder_ShouldBePositive_WhenDescendingIsFalse()
    {
        var attribute = new SortOrderAttribute(2, false);
        Assert.Equal(2, attribute.SortOrder);
    }

    [Fact]
    public void SortOrderAttribute_NegativeSortOrder_ReturnsDescendingTrue()
    {
        var attribute = new SortOrderAttribute(-1);
        Assert.True(attribute.Descending);
    }

    [Fact]
    public void SortOrderAttribute_PositiveSortOrder_ReturnsDescendingFalse()
    {
        var attribute = new SortOrderAttribute(1);
        Assert.False(attribute.Descending);
    }
}
