namespace Serenity.ComponentModel;

public class MultipleFileUploadEditorAttributeTests
{
    [Fact]
    public void EditorType_ShouldBe_MultipleImageUpload()
    {
        var attribute = new MultipleFileUploadEditorAttribute();
        Assert.Equal("MultipleImageUpload", attribute.EditorType);
    }

    [Fact]
    public void AllowNonImage_CanBeSet_ToTrue()
    {
        var attribute = new MultipleFileUploadEditorAttribute()
        {
            AllowNonImage = true
        };
        Assert.True(attribute.AllowNonImage);
    }

    [Fact]
    public void AllowNonImage_IsTrue_ByDefault()
    {
        var attribute = new MultipleFileUploadEditorAttribute();
        Assert.True(attribute.AllowNonImage);
    }

    [Fact]
    public void AllowNonImage_CanBeSet_ToFalse()
    {
        var attribute = new MultipleFileUploadEditorAttribute()
        {
            AllowNonImage = false
        };
        Assert.False(attribute.AllowNonImage);
    }

    [Fact]
    public void JsonEncodeValue_CanBeSet_ToTrue()
    {
        var attribute = new MultipleFileUploadEditorAttribute()
        {
            JsonEncodeValue = true
        };
        Assert.True(attribute.JsonEncodeValue);
    }

    [Fact]
    public void JsonEncodeValue_IsTrue_ByDefault()
    {
        var attribute = new MultipleFileUploadEditorAttribute();
        Assert.True(attribute.JsonEncodeValue);
    }

    [Fact]
    public void JsonEncodeValue_CanBeSet_ToFalse()
    {
        var attribute = new MultipleFileUploadEditorAttribute()
        {
            JsonEncodeValue = false
        };
        Assert.False(attribute.JsonEncodeValue);
    }

    [Fact]
    public void IsMultiple_IsTrue_ByDefault()
    {
        var attribute = new MultipleFileUploadEditorAttribute();
        Assert.True(attribute.IsMultiple);
    }
}



