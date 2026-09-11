using System.IO;

namespace Serenity.Web;

public partial class DefaultUploadProcessorTests
{
    private class TestAVScanner(Action onScan) : IUploadAVScanner
    {
        public void Scan(Stream stream, string filename) => onScan();
    }

    private class ThrowingScaleProcessor : IImageProcessor
    {
        private readonly DefaultImageProcessor inner = new();

        public (int width, int height) GetImageSize(object image) => inner.GetImageSize(image);

        public object? Load(Stream source, out ImageFormatInfo? formatInfo) => inner.Load(source, out formatInfo);

        public void Save(object image, Stream target, string mimeType, ImageEncoderParams encoderParams)
            => inner.Save(image, target, mimeType, encoderParams);

        public object Scale(object image, int width, int height, ImageScaleMode mode, string? backgroundColor, bool inplace)
            => throw new InvalidOperationException("scale failed");
    }

    private class ExposedProcessor(IImageProcessor imageProcessor, IUploadStorage uploadStorage, IUploadValidator uploadValidator)
        : DefaultUploadProcessor(imageProcessor, uploadStorage, uploadValidator)
    {
        public IEnumerable<ScaleImageAsResult> CallAdditionalThumbs(object image, IUploadImageOptions options, string imageFile)
            => CreateAdditionalThumbs(image, options, imageFile);
    }

    [Fact]
    public void Calls_AvScanner_For_NonTemporary_Upload()
    {
        var scanned = false;
        var uploadProcessor = CreateUploadProcessor(avScanner: new TestAVScanner(() => scanned = true));

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("hello"));
        uploadProcessor.Process(stream, "not-temp.txt", new UploadOptions { AllowNonImage = true });

        Assert.True(scanned);
    }

    [Fact]
    public void Writes_New_Temporary_File_Scales_And_Creates_Default_Thumb()
    {
        var uploadProcessor = CreateUploadProcessor();

        var attr = new ImageUploadEditorAttribute
        {
            ScaleWidth = 10,
            ScaleHeight = 10,
            ThumbWidth = 5,
            ThumbHeight = 5
        };

        using var stream = new MemoryStream(CreateImage(20, 20));
        var result = uploadProcessor.Process(stream, "upload.jpg", attr);

        Assert.False(string.IsNullOrEmpty(result.TemporaryFile));
        Assert.Contains(mockFileSystem.AllFiles, f => !f.EndsWith(".meta"));
    }

    [Fact]
    public void Deletes_Temporary_File_When_Processing_Fails()
    {
        var uploadProcessor = CreateUploadProcessor(imageProcessor: new ThrowingScaleProcessor());

        var attr = new ImageUploadEditorAttribute { ScaleWidth = 10, ScaleHeight = 10 };

        using var stream = new MemoryStream(CreateImage(20, 20));
        Assert.Throws<InvalidOperationException>(() => uploadProcessor.Process(stream, "upload.jpg", attr));

        Assert.DoesNotContain(mockFileSystem.AllFiles, f => f.Contains("temporary/") && !f.EndsWith(".meta"));
    }

    [Fact]
    public void CreateAdditionalThumbs_Throws_For_Empty_ImageFile()
    {
        var processor = new ExposedProcessor(new DefaultImageProcessor(),
            new MockUploadStorage(), new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance));

        Assert.Throws<ArgumentNullException>(() =>
            processor.CallAdditionalThumbs(new object(), new UploadOptions(), ""));
    }
}
