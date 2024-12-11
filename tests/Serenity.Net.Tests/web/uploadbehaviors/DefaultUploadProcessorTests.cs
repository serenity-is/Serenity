using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using MockFileData = System.IO.Abstractions.TestingHelpers.MockFileData;

namespace Serenity.Web;

public partial class DefaultUploadProcessorTests
{
    private static byte[] CreateImage(int width, int height, 
        IImageFormat format = null, Configuration configuration = null, Rgba32? color = null)
    {
        using var image = new Image<Rgba32>(configuration ?? Configuration.Default, width, height, color ?? new Rgba32(255, 255, 255));
        using var stream = new System.IO.MemoryStream();

        image.Save(stream, format ?? PngFormat.Instance);

        return stream.ToArray();
    }

    private IUploadStorage storage;
    private MockFileSystem mockFileSystem;

    private DefaultUploadProcessor CreateUploadProcessor(IImageProcessor imageProcessor = null, 
        IUploadStorage uploadStorage = null, 
        IUploadValidator uploadValidator = null,
        ILogger<DefaultUploadProcessor> logger = null,
        IUploadAVScanner avScanner = null)
    {
        storage = uploadStorage ??= MockUploadStorage.Create();
        mockFileSystem = (uploadStorage as MockUploadStorage)?.MockFileSystem;
        imageProcessor ??= new DefaultImageProcessor();
        logger ??= new NullLogger<DefaultUploadProcessor>();
        uploadValidator ??= new DefaultUploadValidator(imageProcessor, NullTextLocalizer.Instance);

        return new DefaultUploadProcessor(imageProcessor,
            storage,
            uploadValidator,
            logger,
            avScanner);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData(".")]
    [InlineData(" . ")]
    [InlineData("..")]
    [InlineData(" .. ")]
    [InlineData("../")]
    [InlineData("../ ")]
    [InlineData(" ../ ")]
    [InlineData("temporary/../")]
    [InlineData("temporary/../ ")]
    [InlineData(" temporary/../ ")]
    [InlineData("..\\")]
    [InlineData("..\\ ")]
    [InlineData(" ..\\ ")]
    [InlineData("temporary\\..\\")]
    [InlineData("temporary\\..\\ ")]
    [InlineData(" temporary\\..\\ ")]
    [InlineData("/")]
    [InlineData(" /")]
    [InlineData(" / ")]
    [InlineData("/a.jpg")]
    [InlineData(" /a.jpg")]
    [InlineData(" /a.jpg ")]
    [InlineData("\\")]
    [InlineData(" \\")]
    [InlineData(" \\ ")]
    [InlineData("\\a.jpg")]
    [InlineData(" \\a.jpg")]
    [InlineData(" \\a.jpg ")]
    public void Throws_WhenFileNameIsNotSecure(string filename)
    {
        var attr = new ImageUploadEditorAttribute();
        var uploadProcessor = CreateUploadProcessor();

        Assert.Throws<ArgumentOutOfRangeException>(() => uploadProcessor.Process(new MemoryStream(), filename, attr));
    }

    [Fact]
    public void Throws_WhenFileNameContainsInvalidChars()
    {
        var invalidChars = System.IO.Path.GetInvalidFileNameChars()
            .Where(x => x != '/' && x != '\\').ToArray();

        var attr = new ImageUploadEditorAttribute();
        var uploadProcessor = CreateUploadProcessor();

        foreach (var invalidChar in invalidChars)
        {
            var testFileNames = new[]
            {
                invalidChar + "",
                " " + invalidChar,
                " " + invalidChar + " ",
                invalidChar + "/a.jpg",
                " " + invalidChar + "/a.jpg",
                " " + invalidChar + "/a.jpg ",
            };

            foreach (var testFileName in testFileNames)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => 
                    uploadProcessor.Process(new MemoryStream(), testFileName, options: attr));
            }
        }
    }

    [Fact]
    public void Throws_WhenFileMinSizeIsSmaller_ThanTheAttribute()
    {
        var uploadProcessor = CreateUploadProcessor();
        var fileName = "temporary/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            MinSize = 10,
        };
        mockFileSystem.AddFile(fileName, string.Empty);

        var ex = Assert.Throws<ValidationError>(() => uploadProcessor.Process(storage.OpenFile(fileName), fileName, attr));

        Assert.Equal("Uploaded file must be at least 0.01 KB!", ex.Message);
    }

    [Fact]
    public void DoesntThrow_WhenFileMinSizeIsBigger_ThanTheAttribute()
    {
        var uploadProcessor = CreateUploadProcessor();
        var fileName = "temporary/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            MinSize = 10,
        };
        mockFileSystem.AddFile(fileName, new MockFileData(CreateImage(1, 1)));

        uploadProcessor.Process(storage.OpenFile(fileName), fileName, attr);
    }

    [Fact]
    public void Throws_WhenFileIsNotValidImage()
    {
        var uploadProcessor = CreateUploadProcessor();
        var fileName = "temporary/test.png";

        var attr = new ImageUploadEditorAttribute();
        mockFileSystem.AddFile(fileName, "\0");

        var ex = Assert.Throws<ValidationError>(() => uploadProcessor.Process(storage.OpenFile(fileName), fileName, attr));

        Assert.Equal($"Enums.{nameof(ImageCheckResult)}.{Enum.GetName(typeof(ImageCheckResult), ImageCheckResult.InvalidImage)}", ex.Message);
    }

    [Fact]
    public void DoesntThrow_WhenFileIsNotValidImage_AndAllowNonImageIsTrue()
    {
        var uploadProcessor = CreateUploadProcessor();
        var fileName = "temporary/test.txt";

        var attr = new ImageUploadEditorAttribute
        {
            AllowNonImage = true
        };
        mockFileSystem.AddFile(fileName, "\0");

        uploadProcessor.Process(storage.OpenFile(fileName), fileName, attr);
    }

    [Fact]
    public void DoesntThrow_WhenFileMinSizeIsZero_OnTheAttribute()
    {
        var uploadProcessor = CreateUploadProcessor();
        var fileName = "temporary/test.txt";

        var attr = new ImageUploadEditorAttribute
        {
            AllowNonImage = true,
            MinSize = 0
        };
        mockFileSystem.AddFile(fileName, "\0");

        uploadProcessor.Process(storage.OpenFile(fileName), fileName, attr);
    }

    [Theory]
    [InlineData(10, 100, 10, 100, 9, 15, ImageCheckResult.WidthTooLow)]
    [InlineData(10, 100, 10, 100, 150, 15, ImageCheckResult.WidthTooHigh)]
    [InlineData(10, 100, 10, 100, 15, 9, ImageCheckResult.HeightTooLow)]
    [InlineData(10, 100, 10, 100, 15, 150, ImageCheckResult.HeightTooHigh)]
    public void Throws_WhenFileDoesntFit_DimensionConstraints(int minWidth, int maxWeight, int minHeight, int maxHeight, int width, int height, ImageCheckResult result)
    {
        var uploadProcessor = CreateUploadProcessor();
        var fileName = "temporary/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            MinWidth = minWidth,
            MaxWidth = maxWeight,
            MinHeight = minHeight,
            MaxHeight = maxHeight
        };

        mockFileSystem.AddFile(fileName, new MockFileData(CreateImage(width, height)));

        var ex = Assert.Throws<ValidationError>(() => uploadProcessor.Process(storage.OpenFile(fileName), fileName, attr));

        Assert.Equal($"Enums.{nameof(ImageCheckResult)}.{Enum.GetName(typeof(ImageCheckResult), result)}", ex.Message);
    }

    [Fact]
    public void ScalesImage_WhenAttributeScaleWidthAndScaleHeight_IsBiggerThanZero()
    {
        var uploadProcessor = CreateUploadProcessor();
        var fileName = "temporary/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            ScaleWidth = 1,
            ScaleHeight = 1,
        };

        mockFileSystem.AddFile(fileName, new MockFileData(CreateImage(2, 2)));
        using var originalImage = Image.Load(mockFileSystem.GetFile(mockFileSystem.AllFiles.Single()).Contents);

        uploadProcessor.Process(storage.OpenFile(fileName), fileName, attr);

        using var scaledImage = Image.Load(mockFileSystem.GetFile(mockFileSystem.AllFiles.Where(x => !x.EndsWith(".meta")).Single()).Contents);

        Assert.Equal(1, scaledImage.Width);
        Assert.Equal(1, scaledImage.Height);

        Assert.Equal(2, originalImage.Width);
        Assert.Equal(2, originalImage.Height);
    }

    [Fact]
    public void DoesntScaleImage_WhenAttributeScaleWidthAndScaleHeight_IsBiggerThanImageSize_AndScaleSmallerIsFalse()
    {
        var uploadProcessor = CreateUploadProcessor();
        var fileName = "temporary/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            ScaleWidth = 10,
            ScaleHeight = 10,
            ScaleSmaller = false
        };

        mockFileSystem.AddFile(fileName, new MockFileData(CreateImage(2, 2)));
        using var originalImage = Image.Load(mockFileSystem.GetFile(mockFileSystem.AllFiles.Single()).Contents);

        uploadProcessor.Process(storage.OpenFile(fileName), fileName, attr);

        using var scaledImage = Image.Load(mockFileSystem.GetFile(mockFileSystem.AllFiles.Where(x => !x.EndsWith(".meta")).Single()).Contents);

        Assert.Equal(2, scaledImage.Width);
        Assert.Equal(2, scaledImage.Height);

        Assert.Equal(2, originalImage.Width);
        Assert.Equal(2, originalImage.Height);
    }

    [Fact]
    public void ScalesImage_WhenAttributeScaleWidthAndScaleHeight_IsBiggerThanImageSize_AndScaleSmallerIsTrue()
    {
        var uploadProcessor = CreateUploadProcessor();
        var fileName = "temporary/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            ScaleWidth = 10,
            ScaleHeight = 10,
            ScaleSmaller = true
        };

        mockFileSystem.AddFile(fileName, new MockFileData(CreateImage(2, 2)));
        using var originalImage = Image.Load(mockFileSystem.GetFile(mockFileSystem.AllFiles.Single()).Contents);

        uploadProcessor.Process(storage.OpenFile(fileName), fileName, attr);

        using var scaledImage = Image.Load(mockFileSystem.GetFile(mockFileSystem.AllFiles.Where(x => !x.EndsWith(".meta")).Single()).Contents);

        Assert.Equal(10, scaledImage.Width);
        Assert.Equal(10, scaledImage.Height);

        Assert.Equal(2, originalImage.Width);
        Assert.Equal(2, originalImage.Height);
    }

    [Fact]
    public void ScalesImage_WithSpecifiedBackgroundColor_WhenItsNotNull()
    {
        var uploadProcessor = CreateUploadProcessor();
        var fileName = "temporary/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            ScaleWidth = 11,
            ScaleHeight = 2,
            ScaleSmaller = true,
            ScaleBackColor = "#FFFFFF",
            ScaleQuality = 100,
            ScaleMode = ImageScaleMode.PreserveRatioWithFill
        };

        mockFileSystem.AddFile(fileName, new MockFileData(CreateImage(1, 1, format: JpegFormat.Instance, color: Rgba32.ParseHex("#000000"))));
        using var originalImage = Image.Load<Rgb24>(mockFileSystem.GetFile(mockFileSystem.AllFiles.Single()).Contents);
    
        uploadProcessor.Process(storage.OpenFile(fileName), fileName, attr);
    
        using var scaledImage = Image.Load<Rgb24>(mockFileSystem.GetFile(mockFileSystem.AllFiles.Where(x => !x.EndsWith(".meta")).Single()).Contents);

        var white = new Rgb24(255, 255, 255);
        var black = new Rgb24(0, 0, 0);
        
        Assert.Equal(white, scaledImage[0, 0]!);
        Assert.Equal(white, scaledImage[0, 1]!);
        Assert.Equal(white, scaledImage[3, 0]!);
        Assert.Equal(white, scaledImage[3, 1]!);
        Assert.Equal(black, scaledImage[4, 0]!);
        Assert.Equal(black, scaledImage[4, 1]!);
        Assert.Equal(black, scaledImage[5, 0]!);
        Assert.Equal(black, scaledImage[5, 1]!);
        Assert.Equal(white, scaledImage[6, 0]!);
        Assert.Equal(white, scaledImage[6, 1]!);
        Assert.Equal(white, scaledImage[10, 0]!);
        Assert.Equal(white, scaledImage[10, 1]!);
    }
    
    [Fact]
    public void ScalesImage_WithBackgroundColor_BlackIfNotSpecified()
    {
        var uploadProcessor = CreateUploadProcessor();
        var fileName = "temporary/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            ScaleWidth = 11,
            ScaleHeight = 2,
            ScaleSmaller = true,
            ScaleQuality = 100,
            ScaleMode = ImageScaleMode.PreserveRatioWithFill
        };

        mockFileSystem.AddFile(fileName, new MockFileData(CreateImage(1, 1, format: JpegFormat.Instance, color: Rgba32.ParseHex("#FFFFFF"))));
        using var originalImage = (Image<Rgb24>)Image.Load(mockFileSystem.GetFile(mockFileSystem.AllFiles.Single()).Contents);
    
        uploadProcessor.Process(storage.OpenFile(fileName), fileName, attr);
    
        using var scaledImage = (Image<Rgb24>)Image.Load(mockFileSystem.GetFile(mockFileSystem.AllFiles.Where(x => !x.EndsWith(".meta")).Single()).Contents);

        var black = new Rgb24(0, 0, 0);
        var white = new Rgb24(255, 255, 255);
        
        Assert.Equal(black, scaledImage[0, 0]!);
        Assert.Equal(black, scaledImage[0, 1]!);
        Assert.Equal(black, scaledImage[3, 0]!);
        Assert.Equal(black, scaledImage[3, 1]!);
        Assert.Equal(white, scaledImage[4, 0]!);
        Assert.Equal(white, scaledImage[4, 1]!);
        Assert.Equal(white, scaledImage[5, 0]!);
        Assert.Equal(white, scaledImage[5, 1]!);
        Assert.Equal(black, scaledImage[6, 0]!);
        Assert.Equal(black, scaledImage[6, 1]!);
        Assert.Equal(black, scaledImage[10, 0]!);
        Assert.Equal(black, scaledImage[10, 1]!);
    }
    
    [Theory]
    [InlineData("-96x96")]
    [InlineData("96x-96")]
    [InlineData("-96x-96")]
    [InlineData("-96x96;100x100")]
    [InlineData("96x-96;100x100")]
    [InlineData("-96x-9;100x100")]
    [InlineData("100x100;-96x96")]
    [InlineData("100x100;96x-96")]
    [InlineData("100x100;-96x-96")]
    public void Throws_When_WhenThumbSizesOnAttr_IsNegative(string thumbSizes)
    {
        var uploadProcessor = CreateUploadProcessor();
        var fileName = "temporary/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            ScaleWidth = 11,
            ScaleHeight = 2,
            ScaleSmaller = true,
            ThumbQuality = 100,
            ThumbMode = ImageScaleMode.PreserveRatioWithFill,
            ThumbSizes = thumbSizes
        };

        mockFileSystem.AddFile(fileName, new MockFileData(CreateImage(1, 1)));
        var originalFile = mockFileSystem.AllFiles.Single();
        using var originalImage = Image.Load(mockFileSystem.GetFile(originalFile).Contents);
    
        Assert.Throws<ArgumentOutOfRangeException>(() => uploadProcessor.Process(storage.OpenFile(fileName), fileName, attr));
    }

    [Fact]
    public void CreatesThumbnails_WhenThumbSizesOnAttr_IsNotNull()
    {
        var uploadProcessor = CreateUploadProcessor();
        var fileName = "temporary/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            ScaleWidth = 11,
            ScaleHeight = 2,
            ScaleSmaller = true,
            ThumbQuality = 100,
            ThumbMode = ImageScaleMode.PreserveRatioWithFill,
            ThumbSizes = "100x100"
        };

        mockFileSystem.AddFile(fileName, new MockFileData(CreateImage(1, 1)));
        var originalFile = mockFileSystem.AllFiles.Single();
        using var originalImage = Image.Load(mockFileSystem.GetFile(originalFile).Contents);
    
        uploadProcessor.Process(storage.OpenFile(fileName), fileName, attr);

        Assert.Single(mockFileSystem.AllFiles.Where(x => !x.EndsWith(".meta")).Except([originalFile]));
    }
    
    [Fact]
    public void SetsThumbnailNames_ToTheirRespectiveWidthAndHeight()
    {
        var uploadProcessor = CreateUploadProcessor();
        var fileName = "temporary/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            ThumbQuality = 100,
            ThumbMode = ImageScaleMode.PreserveRatioWithFill,
            ThumbSizes = "100x100;200x200"
        };

        mockFileSystem.AddFile(fileName, new MockFileData(CreateImage(1, 1)));
        var originalFile = mockFileSystem.AllFiles.Single();

        uploadProcessor.Process(storage.OpenFile(fileName), fileName, attr);

        var originalFileNameWithoutExt = mockFileSystem.GetFileNameWithoutExtension(originalFile);

        Assert.Collection(mockFileSystem.AllFiles.Where(x => !x.EndsWith(".meta")).Except([originalFile]).Select(mockFileSystem.GetFileName).OrderBy(fn => fn),
            x => Assert.Equal($"{originalFileNameWithoutExt}_t100x100.jpg", x),
            x => Assert.Equal($"{originalFileNameWithoutExt}_t200x200.jpg", x));
    }
    
    [Fact]
    public void CreatesThumbnails_WithThumbSizesOnAttr()
    {
        var uploadProcessor = CreateUploadProcessor();
        var fileName = "temporary/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            ThumbQuality = 100,
            ThumbMode = ImageScaleMode.PreserveRatioWithFill,
            ThumbSizes = "100x100,200x200,300x300"
        };

        mockFileSystem.AddFile(fileName, new MockFileData(CreateImage(1, 1)));
        var originalFile = mockFileSystem.AllFiles.Single();
        using var originalImage = Image.Load(mockFileSystem.GetFile(originalFile).Contents);
    
        uploadProcessor.Process(storage.OpenFile(fileName), fileName, attr);


        Assert.Collection(mockFileSystem.AllFiles.Where(x => !x.EndsWith(".meta")).Except([originalFile]).OrderBy(fn => fn),
            file100 =>
            {
                var image = Image.Load(mockFileSystem.GetFile(file100).Contents);
                Assert.Equal(100, image.Width);
                Assert.Equal(100, image.Height);
            },
            file200 =>
            {
                var image = Image.Load(mockFileSystem.GetFile(file200).Contents);
                Assert.Equal(200, image.Width);
                Assert.Equal(200, image.Height);
            }, 
            file300 =>
            {
                var image = Image.Load(mockFileSystem.GetFile(file300).Contents);
                Assert.Equal(300, image.Width);
                Assert.Equal(300, image.Height);
            });
    }
    
    [Fact]
    public void CreatesThumbnails_WithThumbBackColor()
    {
        var uploadProcessor = CreateUploadProcessor();
        var fileName = "temporary/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            ThumbQuality = 100,
            ThumbBackColor = "#FFFFFF",
            ThumbMode = ImageScaleMode.PreserveRatioWithFill,
            ThumbSizes = "100x100,200x200,300x300"
        };

        mockFileSystem.AddFile(fileName, new MockFileData(CreateImage(1, 100, format: JpegFormat.Instance, color: Rgba32.ParseHex("#000000"))));
        var originalFile = mockFileSystem.AllFiles.Single();
        using var originalImage = Image.Load(mockFileSystem.GetFile(originalFile).Contents);
    
        uploadProcessor.Process(storage.OpenFile(fileName), fileName, attr);

        var white = new Rgb24(255, 255, 255);
        var black = new Rgb24(0, 0, 0);
        
        Assert.Collection(mockFileSystem.AllFiles.Where(x => !x.EndsWith(".meta")).Except([originalFile]).OrderBy(fn => fn),
            file100 =>
            {
                var image = Image.Load<Rgb24>(mockFileSystem.GetFile(file100).Contents);
                Assert.Equal(white, image[49, 0]);
                Assert.Equal(black, image[50, 0]);
                Assert.Equal(white, image[51, 0]);
            },
            file200 =>
            {
                var image = Image.Load<Rgb24>(mockFileSystem.GetFile(file200).Contents);
                Assert.Equal(white, image[98, 0]);
                Assert.Equal(black, image[99, 0]);
                Assert.Equal(black, image[100, 0]);
                Assert.Equal(white, image[101, 0]);
            }, 
            file300 =>
            {
                var image = Image.Load<Rgb24>(mockFileSystem.GetFile(file300).Contents);
                Assert.Equal(white, image[147, 0]);
                Assert.Equal(black, image[148, 0]);
                Assert.Equal(black, image[149, 0]);
                Assert.Equal(black, image[150, 0]);
                Assert.Equal(white, image[151, 0]);
            });
    }
}