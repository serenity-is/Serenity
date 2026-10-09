namespace Serenity.ComponentModel;

public class Int64EditorAttributeTests
{
    [Fact]
    public void EditorType_CanBePassed_Int64()
    {
        var attribute = new Int64EditorAttribute();
        Assert.Equal("Int64", attribute.EditorType);
    }

    [Fact]
    public void Constructor_AllowNegativesByDefault_True_ShouldSetAllowNegatives()
    {
        var old = IntegerEditorAttribute.SetLocalAllowNegativesByDefault(true);
        try
        {
            var attribute = new Int64EditorAttribute();
            Assert.True(attribute.AllowNegatives);
        }
        finally
        {
            IntegerEditorAttribute.SetLocalAllowNegativesByDefault(old);
        }
    }

    [Fact]
    public void Constructor_AllowNegativesByDefault_False_ShouldNotSetAllowNegatives()
    {
        var old = IntegerEditorAttribute.SetLocalAllowNegativesByDefault(false);
        try
        {
            var attribute = new Int64EditorAttribute();
            Assert.False(attribute.AllowNegatives);
        }
        finally
        {
            IntegerEditorAttribute.SetLocalAllowNegativesByDefault(old);
        }
    }

    [Fact]
    public void MinValue_CanBeSet_ToLong()
    {
        var attribute = new Int64EditorAttribute()
        {
            MinValue = -9223372036854775807L
        };
        Assert.Equal(-9223372036854775807L, attribute.MinValue);
    }

    [Fact]
    public void MaxValue_CanBeSet_ToLong()
    {
        var attribute = new Int64EditorAttribute()
        {
            MaxValue = 9223372036854775807L
        };
        Assert.Equal(9223372036854775807L, attribute.MaxValue);
    }
}
