namespace Serenity.ComponentModel;

public class DateEditorAttributeTests
{
    [Fact]
    public void EditorType_CanBePassed_Date()
    {
        var attribute = new DateEditorAttribute();
        Assert.Equal("Date", attribute.EditorType);
    }

    [Fact]
    public void MinValue_CanBeSetAndGet()
    {
        var expectedDateTime = new DateTime(2023, 01, 01);
        var attribute = new DateEditorAttribute()
        {
            MinValue = expectedDateTime,
        };
        Assert.Equal(expectedDateTime, attribute.MinValue);
    }

    [Fact]
    public void MaxValue_CanBeSetAndGet()
    {
        var expectedDateTime = new DateTime(2024, 01, 01);
        var attribute = new DateEditorAttribute()
        {
            MaxValue = expectedDateTime,
        };
        Assert.Equal(expectedDateTime, attribute.MaxValue);
    }

    [Fact]
    public void SqlMinMax_CanBeSet_ToTrue()
    {
        var attribute = new DateEditorAttribute()
        {
            SqlMinMax = true
        };
        Assert.True(attribute.SqlMinMax);
    }

    [Fact]
    public void SqlMinMax_CanBeSet_ToFalse()
    {
        var attribute = new DateEditorAttribute()
        {
            SqlMinMax = false
        };
        Assert.False(attribute.SqlMinMax);
    }
}