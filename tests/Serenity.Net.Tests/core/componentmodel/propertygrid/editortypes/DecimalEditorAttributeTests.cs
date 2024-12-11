namespace Serenity.ComponentModel;

public class DecimalEditorAttributeTests
{
    [Fact]
    public void EditorType_CanBePassed_Decimal()
    {
        var attribute = new DecimalEditorAttribute();
        Assert.Equal("Decimal", attribute.EditorType);
    }

    [Fact]
    public void Constructor_AllowNegativesByDefault_True_ShouldSetAllowNegatives()
    {
        DecimalEditorAttribute.AllowNegativesByDefault = true;
        var attribute = new DecimalEditorAttribute();
        Assert.True(attribute.AllowNegatives);
    }

    [Fact]
    public void AllowNegatives_CanBeSet_ToTrue()
    {
        var attribute = new DecimalEditorAttribute()
        {
            AllowNegatives = true
        };
        Assert.True(attribute.AllowNegatives);
    }

    [Fact]
    public void AllowNegatives_CanBeSet_ToFalse()
    {
        var attribute = new DecimalEditorAttribute()
        {
            AllowNegatives = false
        };
        Assert.False(attribute.AllowNegatives);
    }

    [Fact]
    public void Decimals_CanBeSet_ToInt()
    {
        var attribute = new DecimalEditorAttribute()
        {
            Decimals = 1
        };
        Assert.Equal(1, attribute.Decimals);
    }

    [Fact]
    public void MaxValueCanBeSet()
    {
        var attribute = new DecimalEditorAttribute()
        {
            MaxValue = "maxValue"
        };
        Assert.Equal("maxValue", attribute.MaxValue);
    }

    [Fact]
    public void MaxValue_CanBeSet_ToNull()
    {
        var attribute = new DecimalEditorAttribute()
        {
            MaxValue = null
        };
        Assert.Null(attribute.MaxValue);
    }

    [Fact]
    public void MinValue_CanBeSet()
    {
        var attribute = new DecimalEditorAttribute()
        {
            MinValue = "minValue"
        };
        Assert.Equal("minValue", attribute.MinValue);
    }

    [Fact]
    public void MinValue_CanBeSet_ToNull()
    {
        var attribute = new DecimalEditorAttribute()
        {
            MinValue = null
        };
        Assert.Null(attribute.MinValue);
    }

    [Fact]
    public void PadDecimals_CanBeSet_ToTrue()
    {
        var attribute = new DecimalEditorAttribute()
        {
            PadDecimals = true
        };
        Assert.True(attribute.PadDecimals);
    }

    [Fact]
    public void PadDecimals_CanBeSet_ToFalse()
    {
        var attribute = new DecimalEditorAttribute()
        {
            PadDecimals = false
        };
        Assert.False(attribute.PadDecimals);
    }
}