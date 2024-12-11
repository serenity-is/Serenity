namespace Serenity.ComponentModel;

public class EnumEditorAttributeTests
{
    [Fact]
    public void EditorType_CanBePassed_Enum()
    {
        var attribute = new EnumEditorAttribute();
        Assert.Equal("Enum", attribute.EditorType);
    }

    [Fact]
    public void AllowClear_CanBeSet_ToTrue()
    {
        var attribute = new EnumEditorAttribute()
        {
            AllowClear = true
        };
        Assert.True(attribute.AllowClear);
    }

    [Fact]
    public void AllowClear_CanBeSet_ToFalse()
    {
        var attribute = new EnumEditorAttribute()
        {
            AllowClear = false
        };
        Assert.False(attribute.AllowClear);
    }

    [Fact]
    public void Delimited_CanBeSet_ToTrue()
    {
        var attribute = new EnumEditorAttribute()
        {
            Delimited = true
        };
        Assert.True(attribute.Delimited);
    }

    [Fact]
    public void Delimited_CanBeSet_ToFalse()
    {
        var attribute = new EnumEditorAttribute()
        {
            Delimited = false
        };
        Assert.False(attribute.Delimited);
    }

    [Fact]
    public void MinimumResultsForSearch_CanBeSet_ToInt()
    {
        var attribute = new EnumEditorAttribute()
        {
            MinimumResultsForSearch = 1
        };
        Assert.Equal(1, attribute.MinimumResultsForSearch);
    }

    [Fact]
    public void Multiple_CanBeSet_ToTrue()
    {
        var attribute = new EnumEditorAttribute()
        {
            Multiple = true
        };
        Assert.True(attribute.Multiple);
    }

    [Fact]
    public void Multiple_CanBeSet_ToFalse()
    {
        var attribute = new EnumEditorAttribute()
        {
            Multiple = false
        };
        Assert.False(attribute.Multiple);
    }
}
