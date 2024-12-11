namespace Serenity.ComponentModel;

public class UrlFormatterAttributeTests
{
    [Fact]
    public void FormatterType_ShouldBe_Url()
    {
        var attribute = new UrlFormatterAttribute();
        Assert.Equal("Url", attribute.FormatterType);
    }

    [Fact]
    public void DisplayProperty_CanBeSet_ToNull()
    {
        var attribute = new UrlFormatterAttribute()
        {
            DisplayProperty = null
        };
        Assert.Null(attribute.DisplayProperty);
    }

    [Fact]
    public void DisplayProperty_CanBeSet()
    {
        var attribute = new UrlFormatterAttribute()
        {
            DisplayProperty = "DisplayProperty"
        };
        Assert.Equal("DisplayProperty", attribute.DisplayProperty);
    }

    [Fact]
    public void DisplayProperty_IsNull_ByDefault()
    {
        var attribute = new UrlFormatterAttribute();
        Assert.Null(attribute.DisplayProperty);
    }

    [Fact]
    public void DisplayFormat_CanBeSet()
    {
        var attribute = new UrlFormatterAttribute()
        {
            DisplayFormat = "DisplayFormat"
        };
        Assert.Equal("DisplayFormat", attribute.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_CanBeSet_ToNull()
    {
        var attribute = new UrlFormatterAttribute()
        {
            DisplayFormat = null
        };
        Assert.Null(attribute.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_IsNull_ByDefault()
    {
        var attribute = new UrlFormatterAttribute();
        Assert.Null(attribute.DisplayFormat);
    }

    [Fact]
    public void UrlProperty_CanBeSetToNull()
    {
        var attribute = new UrlFormatterAttribute()
        {
            UrlProperty = null
        };
        Assert.Null(attribute.UrlProperty);
    }

    [Fact]
    public void UrlProperty_CanBeSet()
    {
        var attribute = new UrlFormatterAttribute()
        {
            UrlProperty = "UrlProperty"
        };
        Assert.Equal("UrlProperty", attribute.UrlProperty);
    }

    [Fact]
    public void UrlProperty_IsNull_ByDefault()
    {
        var attribute = new UrlFormatterAttribute();
        Assert.Null(attribute.UrlProperty);
    }

    [Fact]
    public void UrlFormat_CanBeSet_ToNull()
    {
        var attribute = new UrlFormatterAttribute()
        {
            UrlFormat = null
        };
        Assert.Null(attribute.UrlFormat);
    }

    [Fact]
    public void UrlFormat_CanBeSet()
    {
        var attribute = new UrlFormatterAttribute()
        {
            UrlFormat = "UrlFormat"
        };
        Assert.Equal("UrlFormat", attribute.UrlFormat);
    }

    [Fact]
    public void UrlFormat_IsNull_ByDefault()
    {
        var attribute = new UrlFormatterAttribute();
        Assert.Null(attribute.UrlFormat);
    }

    [Fact]
    public void Target_CanBeSet_ToNull()
    {
        var attribute = new UrlFormatterAttribute()
        {
            Target = null
        };
        Assert.Null(attribute.Target);
    }
    [Fact]
    public void Target_CanBeSet()
    {
        var attribute = new UrlFormatterAttribute()
        {
            Target = "Target"
        };
        Assert.Equal("Target", attribute.Target);
    }

    [Fact]
    public void Target_IsNull_ByDefault()
    {
        var attribute = new UrlFormatterAttribute();
        Assert.Null(attribute.Target);
    }
}

