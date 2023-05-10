using System.IO;

namespace Serenity.Tests;

public class MockImageProcessor : IImageProcessor
{
    public (int width, int height) GetImageSize(object image)
    {
        return (1000, 1000);
    }

    public object Load(Stream source, out ImageFormatInfo formatInfo)
    {
        formatInfo = new ImageFormatInfo
        {
            MimeType = "image/jpeg",
            FileExtensions = new[] { ".jpg", ".jpeg" }
        };
        return new MockImage();
    }

    public void Save(object image, Stream target, string mimeType, ImageEncoderParams encoderParams)
    {
    }

    public object Scale(object image, int width, int height, ImageScaleMode mode, string backgroundColor, bool inplace)
    {
        return new MockImage();
    }

    private class MockImage
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}