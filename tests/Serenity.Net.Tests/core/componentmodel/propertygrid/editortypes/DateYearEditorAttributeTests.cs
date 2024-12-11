namespace Serenity.ComponentModel;

public class DateYearEditorAttributeTests
{
    [Fact]
    public void EditorType_CanBePassed_DateYear()
    {
        var attribute = new DateYearEditorAttribute();
        Assert.Equal("DateYear", attribute.EditorType);
    }

    [Fact]

    public void MaxYear_CanBeSet()
    {
        var attribute = new DateYearEditorAttribute()
        {
            MaxYear = "maxYear"
        };
        Assert.Equal("maxYear", attribute.MaxYear);
    }

    [Fact]
    public void MaxYear_CanBeSet_ToNull()
    {
        var attribute = new DateYearEditorAttribute()
        {
            MaxYear = null
        };
        Assert.Null(attribute.MaxYear);
    }

    [Fact]
    public void MinYear_CanBeSet()
    {
        var attribute = new DateYearEditorAttribute()
        {
            MinYear = "minYear"
        };
        Assert.Equal("minYear", attribute.MinYear);

    }

    [Fact]
    public void MinYear_CanBeSet_ToNull()
    {
        var attribute = new DateYearEditorAttribute()
        {
            MinYear = null
        };
        Assert.Null(attribute.MinYear);
    }

    [Fact]
    public void Descending_CanBeSet_ToTrue()
    {
        var attribute = new DateYearEditorAttribute()
        {
            Descending = true
        };
        Assert.True(attribute.Descending);
    }

    [Fact]
    public void Descending_CanBeSet_ToFalse()
    {
        var attribute = new DateYearEditorAttribute()
        {
            Descending = false
        };
        Assert.False(attribute.Descending);
    }
}






