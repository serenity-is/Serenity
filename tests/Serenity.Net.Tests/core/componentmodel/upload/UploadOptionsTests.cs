namespace Serenity.Web;

public class UploadOptionsTests
{
    [Fact]
    public void AllowNonImage_IsTrue_ByDefault()
    {
        var attribute = new UploadOptions();
        Assert.True(attribute.AllowNonImage);
    }

    [Fact]
    public void AllowNonImage_CanBeSet_ToTrue()
    {
        var attribute = new UploadOptions()
        {
            AllowNonImage = true
        };
        Assert.True(attribute.AllowNonImage);
    }

    [Fact]
    public void AllowNonImage_CanBeSet_ToFalse()
    {
        var attribute = new UploadOptions()
        {
            AllowNonImage = false
        };
        Assert.False(attribute.AllowNonImage);
    }

    [Fact]
    public void MaxSize_CanBeSet_ToInt()
    {
        var attribute = new UploadOptions()
        {
            MaxSize = 100
        };
        Assert.Equal(100, attribute.MaxSize);
    }

    [Fact]
    public void MaxSize_IsZero_ByDefault()
    {
        var attribute = new UploadOptions();
        Assert.Equal(0, attribute.MaxSize);
    }

    [Fact]
    public void MinSize_CanBeSet_ToInt()
    {
        var attribute = new UploadOptions()
        {
            MinSize = 1
        };
        Assert.Equal(1, attribute.MinSize);
    }

    [Fact]
    public void MinSize_IsZero_ByDefault()
    {
        var attribute = new UploadOptions();
        Assert.Equal(0, attribute.MinSize);
    }

    [Fact]
    public void MaxHeight_CanBeSet_ToInt()
    {
        var attribute = new UploadOptions()
        {
            MaxHeight = 1
        };
        Assert.Equal(1, attribute.MaxHeight);
    }

    [Fact]
    public void MaxHeight_IsZero_ByDefault()
    {
        var attribute = new UploadOptions();
        Assert.Equal(0, attribute.MaxHeight);
    }

    [Fact]
    public void MaxWidth_CanBeSet_ToInt()
    {
        var attribute = new UploadOptions()
        {
            MaxWidth = 1
        };
        Assert.Equal(1, attribute.MaxWidth);
    }

    [Fact]
    public void MaxWidth_IsZero_ByDefault()
    {
        var attribute = new UploadOptions();
        Assert.Equal(0, attribute.MaxWidth);
    }

    [Fact]
    public void MinHeight_CanBeSet_ToInt()
    {
        var attribute = new UploadOptions()
        {
            MinHeight = 1
        };
        Assert.Equal(1, attribute.MinHeight);
    }

    [Fact]
    public void MinHeight_IsZero_ByDefault()
    {
        var attribute = new UploadOptions();
        Assert.Equal(0, attribute.MinHeight);
    }

    [Fact]
    public void MinWidth_CanBeSet_ToInt()
    {
        var attribute = new UploadOptions()
        {
            MinWidth = 1
        };
        Assert.Equal(1, attribute.MinWidth);
    }

    [Fact]
    public void MinWidth_IsZero_ByDefault()
    {
        var attribute = new UploadOptions();
        Assert.Equal(0, attribute.MinWidth);
    }

    [Fact]
    public void ScaleQuality_CanBeSet_ToInt()
    {
        var attribute = new UploadOptions()
        {
            ScaleQuality = 1
        };
        Assert.Equal(1, attribute.ScaleQuality);
    }

    [Fact]
    public void ScaleQuality_Is_ByDefault()
    {
        var attribute = new UploadOptions();
        Assert.Equal(80, attribute.ScaleQuality);
    }

    [Fact]
    public void ScaleWidth_CanBeSet_ToInt()
    {
        var attribute = new UploadOptions()
        {
            ScaleWidth = 1
        };
        Assert.Equal(1, attribute.ScaleWidth);
    }

    [Fact]
    public void ScaleWidth_Is_ByDefault()
    {
        var attribute = new UploadOptions();
        Assert.Equal(0, attribute.ScaleWidth);
    }

    [Fact]
    public void ScaleHeight_CanBeSet_ToInt()
    {
        var attribute = new UploadOptions()
        {
            ScaleHeight = 1
        };
        Assert.Equal(1, attribute.ScaleHeight);
    }

    [Fact]
    public void ScaleHeight_Is_ByDefault()
    {
        var attribute = new UploadOptions();
        Assert.Equal(0, attribute.ScaleHeight);
    }

    [Fact]
    public void ScaleSmaller_CanBeSet_ToTrue()
    {
        var attribute = new UploadOptions()
        {
            ScaleSmaller = true
        };
        Assert.True(attribute.ScaleSmaller);
    }

    [Fact]
    public void ScaleSmaller_CanBeSet_ToFalse()
    {
        var attribute = new UploadOptions()
        {
            ScaleSmaller = false
        };
        Assert.False(attribute.ScaleSmaller);
    }

    [Fact]
    public void ImageScaleMode_CanBeSet_ToPreserveRatioNoFill()
    {
        var attribute = new UploadOptions()
        {
            ScaleMode = ImageScaleMode.PreserveRatioNoFill
        };
        Assert.Equal(ImageScaleMode.PreserveRatioNoFill, attribute.ScaleMode);
    }

    [Fact]
    public void ImageScaleMode_CanBeSet_ToDefaultScaleMode()
    {
        var attribute = new UploadOptions()
        {
            ScaleMode = UploadOptions.DefaultScaleMode
        };
        Assert.Equal(UploadOptions.DefaultScaleMode, attribute.ScaleMode);
    }

    [Fact]
    public void ScaleBackColor_CanBeSet()
    {
        var attribute = new UploadOptions()
        {
            ScaleBackColor = "sometext"
        };
        Assert.Equal("sometext", attribute.ScaleBackColor);
    }

    [Fact]
    public void ScaleBackColor_CanBeSet_ToNull()
    {
        var attribute = new UploadOptions()
        {
            ScaleBackColor = null
        };
        Assert.Null(attribute.ScaleBackColor);
    }

    [Fact]
    public void ThumbHeight_CanBeSet_ToInt()
    {
        var attribute = new UploadOptions()
        {
            ThumbHeight = 1
        };
        Assert.Equal(1, attribute.ThumbHeight);
    }

    [Fact]
    public void ThumbWidth_CanBeSet_ToInt()
    {
        var attribute = new UploadOptions()
        {
            ThumbWidth = 1
        };
        Assert.Equal(1, attribute.ThumbWidth);
    }

    [Fact]
    public void ThumbSizes_CanBeSet()
    {
        var attribute = new UploadOptions()
        {
            ThumbSizes = "sometext"
        };
        Assert.Equal("sometext", attribute.ThumbSizes);
    }

    [Fact]
    public void ThumbSizes_CanBeSet_ToNull()
    {
        var attribute = new UploadOptions()
        {
            ThumbSizes = null
        };
        Assert.Null(attribute.ThumbSizes);
    }

    [Fact]
    public void ThumbMode_CanBeSet_ToEnum()
    {
        var attribute = new UploadOptions()
        {
            ThumbMode = ImageScaleMode.CropSourceImage
        };
        Assert.Equal(ImageScaleMode.CropSourceImage, attribute.ThumbMode);
    }

    [Fact]
    public void ThumbMode_CanBeSet_ToDefaultThumbMode()
    {
        var attribute = new UploadOptions()
        {
            ThumbMode = UploadOptions.DefaultThumbMode
        };
        Assert.Equal(UploadOptions.DefaultThumbMode, attribute.ThumbMode);
    }

    [Fact]
    public void ThumbQuality_CanBeSet_ToDefaultThumbQuality()
    {
        var attribute = new UploadOptions()
        {
            ThumbQuality = UploadOptions.DefaultThumbQuality
        };
        Assert.Equal(UploadOptions.DefaultThumbQuality, attribute.ThumbQuality);
    }

    [Fact]
    public void ThumbBackColor_CanBeSet()
    {
        var attribute = new UploadOptions()
        {
            ThumbBackColor = "sometext"
        };
        Assert.Equal("sometext", attribute.ThumbBackColor);
    }

    [Fact]
    public void ThumbBackColor_CanBeSet_ToNull()
    {
        var attribute = new UploadOptions()
        {
            ThumbBackColor = null
        };
        Assert.Null(attribute.ThumbBackColor);
    }

    [Fact]
    public void JsonEncodeValue_CanBeSet_ToTrue()
    {
        var attribute = new UploadOptions()
        {
            JsonEncodeValue = true
        };
        Assert.True(attribute.JsonEncodeValue);
    }

    [Fact]
    public void JsonEncodeValue_CanBeSet_ToFalse()
    {
        var attribute = new UploadOptions()
        {
            JsonEncodeValue = false
        };
        Assert.False(attribute.JsonEncodeValue);
    }

    [Fact]
    public void OriginalNameProperty_CanBeSet()
    {
        var attribute = new UploadOptions()
        {
            OriginalNameProperty = "sometext"
        };
        Assert.Equal("sometext", attribute.OriginalNameProperty);
    }

    [Fact]
    public void OriginalNameProperty_CanBeSetToNull()
    {
        var attribute = new UploadOptions()
        {
            OriginalNameProperty = null
        };
        Assert.Null(attribute.OriginalNameProperty);
    }

    [Fact]
    public void DisplayFileName_CanBeSet_ToTrue()
    {
        var attribute = new UploadOptions()
        {
            DisplayFileName = true
        };
        Assert.True(attribute.DisplayFileName);
    }

    [Fact]
    public void CopyToHistory_CanBeSet_ToTrue()
    {
        var attribute = new UploadOptions()
        {
            CopyToHistory = true
        };
        Assert.True(attribute.CopyToHistory);
    }

    [Fact]
    public void CopyToHistory_CanBeSet_ToFalse()
    {
        var attribute = new UploadOptions()
        {
            CopyToHistory = false
        };
        Assert.False(attribute.CopyToHistory);
    }

    [Fact]
    public void FilenameFormat_CanBeSet()
    {
        var attribute = new UploadOptions()
        {
            FilenameFormat = "sometext"
        };
        Assert.Equal("sometext", attribute.FilenameFormat);
    }

    [Fact]
    public void FilenameFormat_CanBeSet_ToNull()
    {
        var attribute = new UploadOptions()
        {
            FilenameFormat = null
        };
        Assert.Null(attribute.FilenameFormat);
    }

    [Fact]
    public void AllowedExtensions_CanBeSet()
    {
        var attribute = new UploadOptions()
        {
            AllowedExtensions = "sometext"
        };
        Assert.Equal("sometext", attribute.AllowedExtensions);
    }

    [Fact]
    public void AllowedExtensions_CanBeSet_ToNull()
    {
        var attribute = new UploadOptions()
        {
            AllowedExtensions = null
        };
        Assert.Null(attribute.AllowedExtensions);
    }

    [Fact]
    public void ImageExtensions_CanBeSet()
    {
        var attribute = new UploadOptions()
        {
            ImageExtensions = "sometext"
        };
        Assert.Equal("sometext", attribute.ImageExtensions);
    }

    [Fact]
    public void ImageExtensions_CanBeSet_ToNull()
    {
        var attribute = new UploadOptions()
        {
            ImageExtensions = null
        };
        Assert.Null(attribute.ImageExtensions);
    }

    [Fact]
    public void ImageExtensions_CanBeSet_ToDefaultImageExtension()
    {
        var attribute = new UploadOptions()
        {
            ImageExtensions = UploadOptions.DefaultImageExtensions
        };
        Assert.Equal(UploadOptions.DefaultImageExtensions, attribute.ImageExtensions);
    }

    [Fact]
    public void IgnoreExtensionMismatch_CanBeSet_ToTrue()
    {
        var attribute = new UploadOptions()
        {
            IgnoreExtensionMismatch = true
        };
        Assert.True(attribute.IgnoreExtensionMismatch);
    }

    [Fact]
    public void IgnoreExtensionMismatch_CanBeSet_ToFalse()
    {
        var attribute = new UploadOptions()
        {
            IgnoreExtensionMismatch = false
        };
        Assert.False(attribute.IgnoreExtensionMismatch);
    }

    [Fact]
    public void IgnoreEmptyImage_CanBeSet_ToTrue()
    {
        var attribute = new UploadOptions()
        {
            IgnoreEmptyImage = true
        };
        Assert.True(attribute.IgnoreEmptyImage);
    }

    [Fact]
    public void IgnoreEmptyImage_CanBeSet_ToFalse()
    {
        var attribute = new UploadOptions()
        {
            IgnoreEmptyImage = false
        };
        Assert.False(attribute.IgnoreEmptyImage);
    }

    [Fact]
    public void IgnoreInvalidImage_CanBeSet_ToTrue()
    {
        var attribute = new UploadOptions()
        {
            IgnoreInvalidImage = true
        };
        Assert.True(attribute.IgnoreInvalidImage);
    }

    [Fact]
    public void IgnoreInvalidImage_CanBeSet_ToFalse()
    {
        var attribute = new UploadOptions()
        {
            IgnoreInvalidImage = false
        };
        Assert.False(attribute.IgnoreInvalidImage);
    }
}

