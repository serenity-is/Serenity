namespace Serenity.ComponentModel;

public class TimeEditorAttributeTests
{
    [Fact]
    public void EditorType_ShouldBe_Time()
    {
        var attribute = new TimeEditorAttribute();
        Assert.Equal("Time", attribute.EditorType);
    }

    [Fact]
    public void NoEmptyOption_CanBeSet_ToTrue()
    {
        var attribute = new TimeEditorAttribute()
        {
            NoEmptyOption = true
        };
        Assert.True(attribute.NoEmptyOption);
    }

    [Fact]
    public void NoEmpty_CanBeSet_ToFalse()
    {
        var attribute = new TimeEditorAttribute()
        {
            NoEmptyOption = false
        };
        Assert.False(attribute.NoEmptyOption);
    }

    [Fact]
    public void StartHour_CanBeSet_ToInt()
    {
        var attribute = new TimeEditorAttribute()
        {
            StartHour = 0
        };
        Assert.Equal(0, attribute.StartHour);
    }

    [Fact]
    public void EndHour_CanBeSet_ToInt()
    {
        var attribute = new TimeEditorAttribute()
        {
            EndHour = 0
        };
        Assert.Equal(0, attribute.EndHour);
    }

    [Fact]
    public void IntervalMinutes_CanBeSet_ToInt()
    {
        var attribute = new TimeEditorAttribute()
        {
            IntervalMinutes = 0
        };
        Assert.Equal(0, attribute.IntervalMinutes);
    }

    [Fact]
    public void Multiplier_CanBeSet_ToInt()
    {
        var attribute = new TimeEditorAttribute()
        {
            Multiplier = 0
        };
        Assert.Equal(0, attribute.Multiplier);
    }
}

