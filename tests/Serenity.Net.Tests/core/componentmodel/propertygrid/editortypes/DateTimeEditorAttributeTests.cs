namespace Serenity.ComponentModel;

public class DateTimeEditorAttributeTests
{

    [Fact]
    public void EditorType_CanBePassed_DateTime()
    {
        var attribute = new DateTimeEditorAttribute();
        Assert.Equal("DateTime", attribute.EditorType);
    }

    [Fact]
    public void MinValue_CanBeSet()
    {

        var attribute = new DateTimeEditorAttribute()
        {
            MinValue = new DateTime(2023, 01, 01)
        };
        Assert.Equal(new DateTime(2023, 01, 01), attribute.MinValue);
    }

    [Fact]
    public void MaxValue_CanBeSet()
    {
        var attribute = new DateTimeEditorAttribute()
        {
            MaxValue = new DateTime(2023, 01, 01)
        };
        Assert.Equal(new DateTime(2023, 01, 01), attribute.MaxValue);
    }

    [Fact]
    public void SqlMinMax_CanBeSet_ToTrue()
    {
        var attribute = new DateTimeEditorAttribute()
        {
            SqlMinMax = true
        };
        Assert.True(attribute.SqlMinMax);
    }

    [Fact]
    public void SqlMinMax_CanBeSet_ToFalse()
    {
        var attribute = new DateTimeEditorAttribute()
        {
            SqlMinMax = false
        };
        Assert.False(attribute.SqlMinMax);  
    }

    [Fact]
    public void StartHour_CanBeSet_ToInt()
    {
        var attribute = new DateTimeEditorAttribute()
        {
            StartHour = 2
        };
        Assert.Equal(2, attribute.StartHour);
    }

    [Fact]
    public void EndHour_CanBeSet_ToInt()
    {
        var attribute = new DateTimeEditorAttribute()
        {
            EndHour = 3
        };
        Assert.Equal(3, attribute.EndHour);
    }

    [Fact]
    public void IntervalMinutes_CanBeSet_ToInt()
    {
        var attribute = new DateTimeEditorAttribute()
        {
            IntervalMinutes = 5
        };
        Assert.Equal(5, attribute.IntervalMinutes);

    }

    [Fact]
    public void UseUtc_CanBeSet_ToTrue()
    {
        var attribute = new DateTimeEditorAttribute()
        {
            UseUtc = true
        };
        Assert.True(attribute.UseUtc);
    }

    [Fact]
    public void UseUtc_CanBeSet_ToFalse()
    {
        var attribute = new DateTimeEditorAttribute()
        {
            UseUtc = false
        };
        Assert.False(attribute.UseUtc);
    }
}