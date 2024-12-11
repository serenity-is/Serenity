namespace Serenity.ComponentModel;

public class MultipleImageUploadEditorAttributeTests
{
    [Fact]
    public void EditorType_ShouldBe_ImageUpload()
    {
        var attribute = new MultipleImageUploadEditorAttribute();
        Assert.Equal("MultipleImageUpload", attribute.EditorType);
    }

    [Fact]
    public void AllowNonImage_CanBeSet_ToTrue()
    {
        var attribute = new MultipleImageUploadEditorAttribute()
        {
            AllowNonImage = true
        };
        Assert.True(attribute.AllowNonImage);
    }

    [Fact]
    public void AllowNonImage_IsFalse_ByDefault()
    {
        var attribute = new MultipleImageUploadEditorAttribute();
        Assert.False(attribute.AllowNonImage);
    }

    [Fact]
    public void AllowNonImage_CanBeSet_ToFalse()
    {
        var attribute = new MultipleImageUploadEditorAttribute()
        {
            AllowNonImage = false
        };
        Assert.False(attribute.AllowNonImage);
    }

    [Fact]
    public void JsonEncodeValue_CanBeSet_ToTrue()
    {
        var attribute = new MultipleImageUploadEditorAttribute()
        {
            JsonEncodeValue = true
        };
        Assert.True(attribute.JsonEncodeValue);
    }

    [Fact]
    public void JsonEncodeValue_IsTrue_ByDefault()
    {
        var attribute = new MultipleImageUploadEditorAttribute();
        Assert.True(attribute.JsonEncodeValue);
    }

    [Fact]
    public void JsonEncodeValue_CanBeSet_ToFalse()
    {
        var attribute = new MultipleImageUploadEditorAttribute()
        {
            JsonEncodeValue = false
        };
        Assert.False(attribute.JsonEncodeValue);
    }

    [Fact]
    public void IsMultiple_IsTrue_ByDefault()
    {
        var attribute = new MultipleImageUploadEditorAttribute();
        Assert.True(attribute.IsMultiple);
    }
}
