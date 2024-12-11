namespace Serenity.ComponentModel;

public class BaseUploadEditorAttributeTests
{
    private class MyBaseUploadEditorAttribute : BaseUploadEditorAttribute
    {
        public MyBaseUploadEditorAttribute(string editorType, bool isMultiple) : base(editorType)
        {
            _isMultiple = isMultiple;
        }

        private readonly bool _isMultiple;
        public override bool IsMultiple => _isMultiple;
    }

    [Fact]
    public void DisableDefaultBehavior_CanBeSet_ToTrue()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            DisableDefaultBehavior = true
        };
        Assert.True(attribute.DisableDefaultBehavior);

    }

    [Fact]
    public void DisableDefaultBehavior_CanBeSet_ToFalse()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", false)
        {
            DisableDefaultBehavior = false
        };
        Assert.False(attribute.DisableDefaultBehavior);
    }

    [Fact]
    public void IsMultiple_CanBeSet_ToTrue()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true);
        Assert.True(attribute.IsMultiple);
    }

    [Fact]
    public void IsMultiple_CanBeSet_ToFalse()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", false);
        Assert.False(attribute.IsMultiple);
    }

    [Fact]
    public void UploadIntent_CanBeSet()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            UploadIntent = "sometext"
        };
        Assert.Equal("sometext", attribute.UploadIntent);
    }

    [Fact]
    public void UploadIntent_CanBeSet_ToNull()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            UploadIntent = null
        };
        Assert.Null(attribute.UploadIntent);
    }

    [Fact]
    public void AllowNonImage_CanBeSet_ToTrue()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            AllowNonImage = true
        };
        Assert.True(attribute.AllowNonImage);
    }

    [Fact]
    public void AllowNonImage_CanBeSet_ToFalse()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", false)
        {
            AllowNonImage = false
        };
        Assert.False(attribute.AllowNonImage);
    }

    [Fact]
    public void MaxSize_CanBeSet_ToInt()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            MaxSize = 100
        };
        Assert.Equal(100, attribute.MaxSize);
    }

    [Fact]
    public void MinSize_CanBeSet_ToInt()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            MinSize = 1
        };
        Assert.Equal(1, attribute.MinSize);
    }

    [Fact]
    public void MaxHeight_CanBeSet_ToInt()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            MaxHeight = 100
        };
        Assert.Equal(100, attribute.MaxHeight);
    }

    [Fact]
    public void MaxWidth_CanBeSet_ToInt()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            MaxWidth = 100
        };
        Assert.Equal(100, attribute.MaxWidth);
    }

    [Fact]
    public void MinHeight_CanBeSet_ToInt()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            MinHeight = 100
        };
        Assert.Equal(100, attribute.MinHeight);
    }

    [Fact]
    public void MinWidth_CanBeSet_ToInt()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            MinWidth = 100
        };
        Assert.Equal(100, attribute.MinWidth);
    }

    [Fact]
    public void ScaleQuality_CanBeSet_ToInt()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ScaleQuality = 80
        };
        Assert.Equal(80, attribute.ScaleQuality);
    }

    [Fact]
    public void ScaleWidth_CanBeSet_ToInt()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ScaleWidth = 100
        };
        Assert.Equal(100, attribute.ScaleWidth);
    }

    [Fact]
    public void ScaleHeight_CanBeSet_ToInt()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ScaleHeight = 100
        };
        Assert.Equal(100, attribute.ScaleHeight);
    }

    [Fact]
    public void ScaleSmaller_CanBeSet_ToTrue()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ScaleSmaller = true
        };
        Assert.True(attribute.ScaleSmaller);
    }

    [Fact]
    public void ScaleSmaller_CanBeSet_ToFalse()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ScaleSmaller = false
        };
        Assert.False(attribute.ScaleSmaller);
    }

    [Fact]
    public void ScaleMode_CanBeSet_ToEnum()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ScaleMode = Serenity.Web.ImageScaleMode.StretchToFit
        };
        Assert.Equal(Serenity.Web.ImageScaleMode.StretchToFit, attribute.ScaleMode);
    }

    [Fact]
    public void ScaleMode_CanBeSet_ToDefaultScaleMode()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ScaleMode = UploadOptions.DefaultScaleMode
        };
        Assert.Equal(UploadOptions.DefaultScaleMode, attribute.ScaleMode);
    }

    [Fact]
    public void ScaleBackColor_CanBeSet()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ScaleBackColor = "sometext"
        };
        Assert.Equal("sometext", attribute.ScaleBackColor);
    }

    [Fact]
    public void ScaleBackColor_CanBeSet_ToNull()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ScaleBackColor = null
        };
        Assert.Null(attribute.ScaleBackColor);
    }

    [Fact]
    public void ThumbHeight_CanBeSet_ToInt()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ThumbHeight = 1
        };
        Assert.Equal(1, attribute.ThumbHeight);
    }

    [Fact]
    public void ThumbWidth_CanBeSet_ToInt()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ThumbWidth = 1
        };
        Assert.Equal(1, attribute.ThumbWidth);
    }

    [Fact]
    public void ThumbSizes_CanBeSet()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ThumbSizes = "sometext"
        };
        Assert.Equal("sometext", attribute.ThumbSizes);
    }

    [Fact]
    public void ThumbSizes_CanBeSet_ToNull()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ThumbSizes = null
        };
        Assert.Null(attribute.ThumbSizes);
    }

    [Fact]
    public void ThumbMode_CanBeSet_ToEnum()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ThumbMode = ImageScaleMode.CropSourceImage
        };
        Assert.Equal(ImageScaleMode.CropSourceImage, attribute.ThumbMode);
    }

    [Fact]
    public void ThumbMode_CanBeSet_ToDefaultThumbMode()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ThumbMode = UploadOptions.DefaultThumbMode
        };
        Assert.Equal(UploadOptions.DefaultThumbMode, attribute.ThumbMode);
    }

    [Fact]
    public void ThumbQuality_CanBeSet_ToDefaultThumbQuality()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ThumbQuality = UploadOptions.DefaultThumbQuality
        };
        Assert.Equal(UploadOptions.DefaultThumbQuality, attribute.ThumbQuality);
    }

    [Fact]
    public void ThumbBackColor_CanBeSet()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ThumbBackColor = "sometext"
        };
        Assert.Equal("sometext", attribute.ThumbBackColor);
    }

    [Fact]
    public void ThumbBackColor_CanBeSet_ToNull()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ThumbBackColor = null
        };
        Assert.Null(attribute.ThumbBackColor);
    }

    [Fact]
    public void JsonEncodeValue_CanBeSet_ToTrue()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            JsonEncodeValue = true
        };
        Assert.True(attribute.JsonEncodeValue);
    }

    [Fact]
    public void JsonEncodeValue_CanBeSet_ToFalse()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", false)
        {
            JsonEncodeValue = false
        };
        Assert.False(attribute.JsonEncodeValue);
    }

    [Fact]
    public void OriginalNameProperty_CanBeSet()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            OriginalNameProperty = "sometext"
        };
        Assert.Equal("sometext", attribute.OriginalNameProperty);
    }

    [Fact]
    public void OriginalNameProperty_CanBeSetToNull()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            OriginalNameProperty = null
        };
        Assert.Null(attribute.OriginalNameProperty);
    }

    [Fact]
    public void DisplayFileName_CanBeSet_ToTrue()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            DisplayFileName = true
        };
        Assert.True(attribute.DisplayFileName);
    }
    [Fact]
    public void CopyToHistory_CanBeSet_ToTrue()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            CopyToHistory = true
        };
        Assert.True(attribute.CopyToHistory);
    }

    [Fact]
    public void CopyToHistory_CanBeSet_ToFalse()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", false)
        {
            CopyToHistory = false
        };
        Assert.False(attribute.CopyToHistory);
    }

    [Fact]
    public void FilenameFormat_CanBeSet()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            FilenameFormat = "sometext"
        };
        Assert.Equal("sometext", attribute.FilenameFormat);
    }

    [Fact]
    public void FilenameFormat_CanBeSet_ToNull()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            FilenameFormat = null
        };
        Assert.Null(attribute.FilenameFormat);
    }

    [Fact]
    public void AllowedExtensions_CanBeSet()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            AllowedExtensions = "sometext"
        };
        Assert.Equal("sometext", attribute.AllowedExtensions);
    }

    [Fact]
    public void AllowedExtensions_CanBeSet_ToNull()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            AllowedExtensions = null
        };
        Assert.Null(attribute.AllowedExtensions);
    }

    [Fact]
    public void ImageExtensions_CanBeSet()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ImageExtensions = "sometext"
        };
        Assert.Equal("sometext", attribute.ImageExtensions);
    }

    [Fact]
    public void ImageExtensions_CanBeSet_ToNull()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ImageExtensions = null
        };
        Assert.Null(attribute.ImageExtensions);
    }

    [Fact]
    public void ImageExtensions_CanBeSet_ToDefaultImageExtension()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            ImageExtensions = UploadOptions.DefaultImageExtensions
        };
        Assert.Equal(UploadOptions.DefaultImageExtensions, attribute.ImageExtensions);
    }

    [Fact]
    public void IgnoreExtensionMismatch_CanBeSet_ToTrue()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            IgnoreExtensionMismatch = true
        };
        Assert.True(attribute.IgnoreExtensionMismatch);
    }

    [Fact]
    public void IgnoreExtensionMismatch_CanBeSet_ToFalse()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", false)
        {
            IgnoreExtensionMismatch = false
        };
        Assert.False(attribute.IgnoreExtensionMismatch);
    }

    [Fact]
    public void IgnoreEmptyImage_CanBeSet_ToTrue()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            IgnoreEmptyImage = true
        };
        Assert.True(attribute.IgnoreEmptyImage);
    }

    [Fact]
    public void IgnoreEmptyImage_CanBeSet_ToFalse()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", false)
        {
            IgnoreEmptyImage = false
        };
        Assert.False(attribute.IgnoreEmptyImage);
    }

    [Fact]
    public void IgnoreInvalidImage_CanBeSet_ToTrue()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", true)
        {
            IgnoreInvalidImage = true
        };
        Assert.True(attribute.IgnoreInvalidImage);
    }

    [Fact]
    public void IgnoreInvalidImage_CanBeSet_ToFalse()
    {
        var attribute = new MyBaseUploadEditorAttribute("text", false)
        {
            IgnoreInvalidImage = false
        };
        Assert.False(attribute.IgnoreInvalidImage);
    }
}





