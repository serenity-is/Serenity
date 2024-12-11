namespace Serenity.ComponentModel;

public class ImageUploadEditorAttributeTests
{
    [Fact]
    public void EditorType_ShouldBe_ImageUpload()
    {
        var attribute = new ImageUploadEditorAttribute();
        Assert.Equal("ImageUpload", attribute.EditorType);
    }

    [Fact]
    public void AllowNonImage_CanBeSet_ToTrue()
    {
        var attribute = new ImageUploadEditorAttribute()
        {
            AllowNonImage = true
        };
        Assert.True(attribute.AllowNonImage);
    }

    [Fact]
    public void AllowNonImage_IsFalse_ByDefault()
    {
        var attribute = new ImageUploadEditorAttribute();
        Assert.False(attribute.AllowNonImage);
    }

    [Fact]
    public void AllowNonImage_CanBeSet_ToFalse()
    {
        var attribute = new ImageUploadEditorAttribute()
        {
            AllowNonImage = false
        };
        Assert.False(attribute.AllowNonImage);
    }

    [Fact]
    public void IsMultiple_Isfalse_ByDefault()
    {
        var attribute = new ImageUploadEditorAttribute();
        Assert.False(attribute.IsMultiple);
    }
}
