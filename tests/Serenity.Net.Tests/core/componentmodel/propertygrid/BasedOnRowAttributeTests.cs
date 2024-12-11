namespace Serenity.ComponentModel;

public class BasedOnRowAttributeTests
{
    [Fact]
    public void RowType_CanBePassed_AsTypeOfBool()
    { 
        var attribute = new BasedOnRowAttribute(typeof(bool));
        Assert.Equal(typeof(bool), attribute.RowType);
    }

    [Fact]
    public void RowType_CanBePassed_AsTypeOfInt()
    {
        var attribute = new BasedOnRowAttribute(typeof(int));
        Assert.Equal(typeof(int), attribute.RowType);
    }

    [Fact]
    public void RowType_CanBePassed_AsTypeOfEnum()
    {
        var attribute = new BasedOnRowAttribute(typeof(Enum));
        Assert.Equal(typeof(Enum), attribute.RowType);
    }

    [Fact]
    public void RowType_CanBePassed_AsTypeOfString()
    {
        var attribute = new BasedOnRowAttribute(typeof(string));
        Assert.Equal(typeof(string), attribute.RowType);
    }

    [Fact]
    public void CheckNames_CanBeSet_ToTrue()
    {
        var attribute = new BasedOnRowAttribute(typeof(bool))
        {
            CheckNames = true
        };
        Assert.True(attribute.CheckNames);
    }

    [Fact]
    public void CheckNames_CanBeSet_ToFalse()
    {
        var attribute = new BasedOnRowAttribute(typeof(bool))
        {
            CheckNames = false
        };
        Assert.False(attribute.CheckNames);
    }
}
