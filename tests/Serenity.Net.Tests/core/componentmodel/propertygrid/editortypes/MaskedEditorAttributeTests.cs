namespace Serenity.ComponentModel;

public class MaskedEditorAttributeTests
{
    [Fact]
    public void EditorType_ShouldBe_Masked()
    {
        var attribute = new MaskedEditorAttribute();
        Assert.Equal("Masked", attribute.EditorType);
    }

    [Fact]
    public void Mask_CanBeSet()
    {
        var attribute = new MaskedEditorAttribute()
        {
            Mask = "Mask"
        };
        Assert.Equal("Mask", attribute.Mask);
    }

    [Fact]
    public void Mask_CanBeSet_ToNull()
    {
        var attribute = new MaskedEditorAttribute()
        {
            Mask = null
        };
        Assert.Null(attribute.Mask);
    }

    [Fact]
    public void Placeholder_CanBeSet()
    {
        var attribute = new MaskedEditorAttribute()
        {
            Placeholder = "sometext"
        };
        Assert.Equal("sometext", attribute.Placeholder);
    }

    [Fact]
    public void Placeholder_CanBeSet_ToNull()
    {
        var attribute = new MaskedEditorAttribute()
        {
            Placeholder = null
        };
        Assert.Null(attribute.Placeholder);
    }
}
