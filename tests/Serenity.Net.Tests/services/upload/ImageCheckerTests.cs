using System.IO;

namespace Serenity.Web;

public class ImageCheckerTests
{
    private readonly DefaultImageProcessor processor = new();

    [Fact]
    public void CheckStream_Returns_StreamReadError_For_Empty()
    {
        var result = new ImageChecker().CheckStream(
            new MemoryStream(), processor, false, out var image, out var formatInfo);

        Assert.Equal(ImageCheckResult.StreamReadError, result);
        Assert.Null(image);
        Assert.Null(formatInfo);
    }

    [Fact]
    public void CheckStream_Returns_DataSizeTooHigh()
    {
        var result = new ImageChecker { MaxDataSize = 1 }.CheckStream(
            new MemoryStream(new byte[5]), processor, false, out _, out _);

        Assert.Equal(ImageCheckResult.DataSizeTooHigh, result);
    }

    [Fact]
    public void CheckStream_Returns_InvalidImage()
    {
        var result = new ImageChecker().CheckStream(
            new MemoryStream([1, 2, 3]), processor, false, out _, out _);

        Assert.Equal(ImageCheckResult.InvalidImage, result);
    }

    [Fact]
    public void CheckStream_Returns_StreamReadError_For_NonSeekable()
    {
        var result = new ImageChecker().CheckStream(
            new ThrowingStream(), new MockImageProcessor(), false, out _, out _);

        Assert.Equal(ImageCheckResult.StreamReadError, result);
    }

    [Fact]
    public void CheckSizeConstraints_Returns_ImageIsEmpty()
    {
        Assert.Equal(ImageCheckResult.ImageIsEmpty, new ImageChecker().CheckSizeConstraints(0, 5));
        Assert.Equal(ImageCheckResult.ImageIsEmpty, new ImageChecker().CheckSizeConstraints(5, 0));
    }

    [Fact]
    public void CheckSizeConstraints_Returns_Size_And_Width_Mismatch()
    {
        var sizeMismatch = new ImageChecker { MinWidth = 10, MaxWidth = 10, MinHeight = 20, MaxHeight = 20 };
        Assert.Equal(ImageCheckResult.SizeMismatch, sizeMismatch.CheckSizeConstraints(5, 5));

        var widthMismatch = new ImageChecker { MinWidth = 10, MaxWidth = 10, MinHeight = 20, MaxHeight = 30 };
        Assert.Equal(ImageCheckResult.WidthMismatch, widthMismatch.CheckSizeConstraints(5, 25));
    }

    [Fact]
    public void CheckSizeConstraints_Returns_HeightMismatch_And_TooLow()
    {
        var heightMismatch = new ImageChecker { MinWidth = 10, MaxWidth = 20, MinHeight = 20, MaxHeight = 20 };
        Assert.Equal(ImageCheckResult.HeightMismatch, heightMismatch.CheckSizeConstraints(15, 5));

        var heightTooLow = new ImageChecker { MinWidth = 10, MaxWidth = 20, MinHeight = 20, MaxHeight = 30 };
        Assert.Equal(ImageCheckResult.HeightTooLow, heightTooLow.CheckSizeConstraints(15, 5));
    }

    [Fact]
    public void FormatErrorMessage_Formats_All_Results()
    {
        var checker = new ImageChecker { MaxDataSize = 100, MinWidth = 10, MinHeight = 10, MaxWidth = 50, MaxHeight = 50 };

        Assert.NotNull(checker.FormatErrorMessage(ImageCheckResult.DataSizeTooHigh, NullTextLocalizer.Instance));
        Assert.NotNull(checker.FormatErrorMessage(ImageCheckResult.SizeMismatch, NullTextLocalizer.Instance));
        Assert.NotNull(checker.FormatErrorMessage(ImageCheckResult.WidthTooLow, NullTextLocalizer.Instance));
        Assert.NotNull(checker.FormatErrorMessage(ImageCheckResult.HeightTooLow, NullTextLocalizer.Instance));
        Assert.NotNull(checker.FormatErrorMessage(ImageCheckResult.WidthMismatch, NullTextLocalizer.Instance));
        Assert.NotNull(checker.FormatErrorMessage(ImageCheckResult.HeightMismatch, NullTextLocalizer.Instance));
        Assert.NotNull(checker.FormatErrorMessage(ImageCheckResult.WidthTooHigh, NullTextLocalizer.Instance));
        Assert.NotNull(checker.FormatErrorMessage(ImageCheckResult.HeightTooHigh, NullTextLocalizer.Instance));
        Assert.NotNull(checker.FormatErrorMessage(ImageCheckResult.Valid, NullTextLocalizer.Instance));
    }

    private class ThrowingStream : MemoryStream
    {
        public override long Seek(long offset, SeekOrigin loc) => throw new IOException("nope");
        public override long Length => throw new IOException("nope");
    }
}
