#pragma warning disable CS0618
using System.IO;
using System.Text;

namespace Serenity.Web;

public class UploadProcessorTests
{
    [Fact]
    public void ProcessStream_Throws_ForNullContent()
    {
        var processor = new UploadProcessor(new MockUploadStorage());

        Assert.Throws<ArgumentNullException>(() =>
            processor.ProcessStream(null!, ".png", NullTextLocalizer.Instance));
    }

    [Fact]
    public void ProcessStream_Processes_NonImage()
    {
        var storage = new MockUploadStorage();
        var processor = new UploadProcessor(storage);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("hello"));
        var result = processor.ProcessStream(stream, ".txt", NullTextLocalizer.Instance,
            new UploadOptions { AllowNonImage = true });

        Assert.True(result);
        Assert.False(processor.IsImage);
        Assert.Equal(5, processor.FileSize);
        Assert.False(string.IsNullOrEmpty(processor.TemporaryFile));
    }

    [Fact]
    public void ProcessStream_Uses_ThumbProperties_WhenOptionsNull()
    {
        var storage = new MockUploadStorage();
        var processor = new UploadProcessor(storage)
        {
            ThumbWidth = 10,
            ThumbHeight = 10,
            ThumbQuality = 50,
            ThumbBackColor = "#FFFFFF",
            ThumbScaleMode = ImageScaleMode.PreserveRatioWithFill
        };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("data"));
        var result = processor.ProcessStream(stream, ".txt", NullTextLocalizer.Instance,
            new UploadOptions { AllowNonImage = true });

        Assert.True(result);
    }
}
