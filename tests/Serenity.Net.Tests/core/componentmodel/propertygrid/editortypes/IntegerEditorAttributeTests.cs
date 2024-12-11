namespace Serenity.ComponentModel;

public class IntegerEditorAttributeTests
{
    [Fact]
    public void EditorType_CanBePassed_Integer()
    {
        var attribute = new IntegerEditorAttribute();
        Assert.Equal("Integer", attribute.EditorType);
    }

    [Fact]
    public void Constructor_AllowNegativesByDefault_True_ShouldSetAllowNegatives()
    {
        IntegerEditorAttribute.AllowNegativesByDefault = true;
        var attribute = new IntegerEditorAttribute();
        Assert.True(attribute.AllowNegatives);
    }

    [Fact]
    public void Constructor_AllowNegativesByDefault_False_ShouldNotSetAllowNegatives()
    {
        IntegerEditorAttribute.AllowNegativesByDefault = false;
        var attribute = new IntegerEditorAttribute();
        Assert.False(attribute.AllowNegatives);
    }

    [Fact]
    public void MinValue_CanBeSet_ToLong()
    {
        var attribute = new IntegerEditorAttribute()
        {
            MinValue = -9223372036854775807L
        };
        Assert.Equal(-9223372036854775807L, attribute.MinValue);
    }

    [Fact]
    public void MaxValue_CanBeSet_ToLong()
    {
        var attribute = new IntegerEditorAttribute()
        {
            MaxValue = 9223372036854775807L
        };
        Assert.Equal(9223372036854775807L, attribute.MaxValue);
    }
}

