namespace Serenity.ComponentModel;

public class FileUploadEditorAttributeTests
{
    [Fact]
    public void EditorType_ShouldBe_ImageUpload()
    {
        var attribute = new FileUploadEditorAttribute();
        Assert.Equal("ImageUpload", attribute.EditorType);
    }

    [Fact]
    public void AllowNonImage_CanBeSet_ToTrue()
    {
        var attribute = new FileUploadEditorAttribute()
        {
            AllowNonImage = true
        };
        Assert.True(attribute.AllowNonImage);
    }

    [Fact]
    public void AllowNonImage_IsTrue_ByDefault()
    {
        var attribute = new FileUploadEditorAttribute();
        Assert.True(attribute.AllowNonImage);
    }

    [Fact]
    public void AllowNonImage_CanBeSet_ToFalse()
    {
        var attribute = new FileUploadEditorAttribute()
        {
            AllowNonImage = false
        };
        Assert.False(attribute.AllowNonImage);
    }

    [Fact]
    public void IsMultiple_Isfalse_ByDefault()
    {
        var attribute = new FileUploadEditorAttribute();
        Assert.False(attribute.IsMultiple);
    }
}
