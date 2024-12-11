namespace Serenity.ComponentModel;

public class TimeSpanEditorAttributeTests
{
    [Fact]
    public void EditorType_ShouldBe_TimeSpan()
    {
        var attribute = new TimeSpanEditorAttribute();
        Assert.Equal("TimeSpan", attribute.EditorType);
    }

    [Fact]
    public void NoEmptyOption_CanBeSet_ToTrue()
    {
        var attribute = new TimeSpanEditorAttribute()
        {
            NoEmptyOption = true
        };
        Assert.True(attribute.NoEmptyOption);
    }

    [Fact]
    public void NoEmpty_CanBeSet_ToFalse()
    {
        var attribute = new TimeSpanEditorAttribute()
        {
            NoEmptyOption = false
        };
        Assert.False(attribute.NoEmptyOption);
    }

    [Fact]
    public void StartHour_CanBeSet_ToInt()
    {
        var attribute = new TimeSpanEditorAttribute()
        {
            StartHour = 0
        };
        Assert.Equal(0, attribute.StartHour);
    }

    [Fact]
    public void EndHour_CanBeSet_ToInt()
    {
        var attribute = new TimeSpanEditorAttribute()
        {
            EndHour = 0
        };
        Assert.Equal(0, attribute.EndHour);
    }

    [Fact]
    public void IntervalMinutes_CanBeSet_ToInt()
    {
        var attribute = new TimeSpanEditorAttribute()
        {
            IntervalMinutes = 0
        };
        Assert.Equal(0, attribute.IntervalMinutes);
    }
}
