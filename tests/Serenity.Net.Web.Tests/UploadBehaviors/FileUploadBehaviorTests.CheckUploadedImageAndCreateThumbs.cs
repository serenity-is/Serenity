using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using MockFileData = System.IO.Abstractions.TestingHelpers.MockFileData;

namespace Serenity.Tests.Web;

public partial class FileUploadBehaviorTests
{
    [Fact]
    public void Throws_WhenStorageIsNull()
    {
        var attr = new ImageUploadEditorAttribute();
        var temporaryFile = "temporary/test.jpg";

        Assert.Throws<ArgumentNullException>("storage", () => FileUploadBehavior.CheckUploadedImageAndCreateThumbs(uploadOptions: attr,
            imageProcessor: new DefaultImageProcessor(),
            storage: null,
            temporaryFile: ref temporaryFile,
            validator: new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance)));
    }

    [Theory]
    [InlineData(null)]
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
    public void Throws_WhenFileNameIsNotSecure(string fileName)
    {
        var attr = new ImageUploadEditorAttribute();
        var mockStorage = MockUploadStorage.Create();

        Assert.Throws<ArgumentOutOfRangeException>(() => FileUploadBehavior.CheckUploadedImageAndCreateThumbs(uploadOptions: attr,
            imageProcessor: new DefaultImageProcessor(),
            storage: mockStorage,
            temporaryFile: ref fileName,
            validator: new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance)));
    }

    [Fact]
    public void Throws_WhenFileNameContainsInvalidChars()
    {
        var invalidChars = System.IO.Path.GetInvalidFileNameChars()
            .Where(x => x != '/' && x != '\\').ToArray();

        var attr = new ImageUploadEditorAttribute();
        var mockStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockStorage.MockFileSystem;


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
                var fileName = testFileName;
                mockFileSystem.AddFile(testFileName, string.Empty);

                Assert.Throws<ArgumentOutOfRangeException>(() => FileUploadBehavior.CheckUploadedImageAndCreateThumbs(uploadOptions: attr,
                    imageProcessor: new DefaultImageProcessor(),
                    storage: mockStorage,
                    temporaryFile: ref fileName,
                    validator: new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance)));
            }
        }
    }

    [Fact]
    public void Throws_WhenFileMinSizeIsSmaller_ThanTheAttribute()
    {
        var mockStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockStorage.MockFileSystem;
        var fileName = "temporary/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            MinSize = 10,
        };
        mockFileSystem.AddFile(fileName, string.Empty);

        var ex = Assert.Throws<ValidationError>(() => FileUploadBehavior.CheckUploadedImageAndCreateThumbs(uploadOptions: attr,
            imageProcessor: new DefaultImageProcessor(),
            storage: mockStorage,
            temporaryFile: ref fileName,
            validator: new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance)));

        Assert.Equal("Uploaded file must be at least 0.01 KB!", ex.Message);
    }

    [Fact]
    public void DoesntThrows_WhenFileMinSizeIsBigger_ThanTheAttribute()
    {
        var mockStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockStorage.MockFileSystem;
        var fileName = "temporary/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            MinSize = 10,
        };
        mockFileSystem.AddFile(fileName, new MockFileData(CreateImage(1, 1)));

        FileUploadBehavior.CheckUploadedImageAndCreateThumbs(uploadOptions: attr,
            imageProcessor: new DefaultImageProcessor(),
            storage: mockStorage,
            temporaryFile: ref fileName,
            validator: new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance));
    }

    [Fact]
    public void Throws_WhenFileIsNotValidImage()
    {
        var mockStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockStorage.MockFileSystem;
        var fileName = "temporary/test.png";

        var attr = new ImageUploadEditorAttribute();
        mockFileSystem.AddFile(fileName, "\0");

        var ex = Assert.Throws<ValidationError>(() => FileUploadBehavior.CheckUploadedImageAndCreateThumbs(uploadOptions: attr,
            imageProcessor: new DefaultImageProcessor(),
            storage: mockStorage,
            temporaryFile: ref fileName,
            validator: new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance)));

        Assert.Equal($"Enums.{nameof(ImageCheckResult)}.{Enum.GetName(typeof(ImageCheckResult), ImageCheckResult.InvalidImage)}", ex.Message);
    }

    [Fact]
    public void DoesntThrows_WhenFileIsNotValidImage_AndAllowNonImageIsTrue()
    {
        var mockStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockStorage.MockFileSystem;
        var fileName = "temporary/test.txt";

        var attr = new ImageUploadEditorAttribute
        {
            AllowNonImage = true
        };
        mockFileSystem.AddFile(fileName, "\0");

        FileUploadBehavior.CheckUploadedImageAndCreateThumbs(uploadOptions: attr,
            imageProcessor: new DefaultImageProcessor(),
            storage: mockStorage,
            temporaryFile: ref fileName,
            validator: new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance));
    }

    [Fact]
    public void DoesntThrows_WhenFileMinSizeIsZero_OnTheAttribute()
    {
        var mockStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockStorage.MockFileSystem;
        var fileName = "temporary/test.txt";

        var attr = new ImageUploadEditorAttribute
        {
            AllowNonImage = true,
            MinSize = 0
        };
        mockFileSystem.AddFile(fileName, "\0");

        FileUploadBehavior.CheckUploadedImageAndCreateThumbs(uploadOptions: attr,
            imageProcessor: new DefaultImageProcessor(),
            storage: mockStorage,
            temporaryFile: ref fileName,
            validator: new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance));
    }

    [Theory]
    [InlineData(10, 100, 10, 100, 9, 15, ImageCheckResult.WidthTooLow)]
    [InlineData(10, 100, 10, 100, 150, 15, ImageCheckResult.WidthTooHigh)]
    [InlineData(10, 100, 10, 100, 15, 9, ImageCheckResult.HeightTooLow)]
    [InlineData(10, 100, 10, 100, 15, 150, ImageCheckResult.HeightTooHigh)]
    public void Throws_WhenFileDoesntFit_DimensionConstraints(int minWidth, int maxWeight, int minHeight, int maxHeight, int width, int height, ImageCheckResult result)
    {
        var mockStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockStorage.MockFileSystem;
        var fileName = "temporary/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            MinWidth = minWidth,
            MaxWidth = maxWeight,
            MinHeight = minHeight,
            MaxHeight = maxHeight
        };

        mockFileSystem.AddFile(fileName, new MockFileData(CreateImage(width, height)));

        var ex = Assert.Throws<ValidationError>(() => FileUploadBehavior.CheckUploadedImageAndCreateThumbs(uploadOptions: attr,
            imageProcessor: new DefaultImageProcessor(),
            storage: mockStorage,
            temporaryFile: ref fileName,
            validator: new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance)));

        Assert.Equal($"Enums.{nameof(ImageCheckResult)}.{Enum.GetName(typeof(ImageCheckResult), result)}", ex.Message);
    }

    [Fact]
    public void ScalesImage_WhenAttributeScaleWidthAndScaleHeight_IsBiggerThanZero()
    {
        var mockStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockStorage.MockFileSystem;
        var fileName = "upload/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            ScaleWidth = 1,
            ScaleHeight = 1,
        };

        mockFileSystem.AddFile(fileName, new MockFileData(CreateImage(2, 2)));
        using var originalImage = Image.Load(mockFileSystem.GetFile(mockFileSystem.AllFiles.Single()).Contents);

        FileUploadBehavior.CheckUploadedImageAndCreateThumbs(uploadOptions: attr,
            imageProcessor: new DefaultImageProcessor(),
            storage: mockStorage,
            temporaryFile: ref fileName,
            validator: new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance));

        using var scaledImage = Image.Load(mockFileSystem.GetFile(mockFileSystem.AllFiles.Single()).Contents);

        Assert.Equal(1, scaledImage.Width);
        Assert.Equal(1, scaledImage.Height);

        Assert.Equal(2, originalImage.Width);
        Assert.Equal(2, originalImage.Height);
    }

    [Fact]
    public void DoesntScalesImage_WhenAttributeScaleWidthAndScaleHeight_IsBiggerThanImageSize_AndScaleSmallerIsFalse()
    {
        var mockStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockStorage.MockFileSystem;
        var fileName = "upload/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            ScaleWidth = 10,
            ScaleHeight = 10,
            ScaleSmaller = false
        };

        mockFileSystem.AddFile(fileName, new MockFileData(CreateImage(2, 2)));
        using var originalImage = Image.Load(mockFileSystem.GetFile(mockFileSystem.AllFiles.Single()).Contents);

        FileUploadBehavior.CheckUploadedImageAndCreateThumbs(uploadOptions: attr,
            imageProcessor: new DefaultImageProcessor(),
            storage: mockStorage,
            temporaryFile: ref fileName,
            validator: new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance));

        using var scaledImage = Image.Load(mockFileSystem.GetFile(mockFileSystem.AllFiles.Single()).Contents);

        Assert.Equal(2, scaledImage.Width);
        Assert.Equal(2, scaledImage.Height);

        Assert.Equal(2, originalImage.Width);
        Assert.Equal(2, originalImage.Height);
    }

    [Fact]
    public void ScalesImage_WhenAttributeScaleWidthAndScaleHeight_IsBiggerThanImageSize_AndScaleSmallerIsTrue()
    {
        var mockStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockStorage.MockFileSystem;
        var fileName = "upload/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            ScaleWidth = 10,
            ScaleHeight = 10,
            ScaleSmaller = true
        };

        mockFileSystem.AddFile(fileName, new MockFileData(CreateImage(2, 2)));
        using var originalImage = Image.Load(mockFileSystem.GetFile(mockFileSystem.AllFiles.Single()).Contents);

        FileUploadBehavior.CheckUploadedImageAndCreateThumbs(uploadOptions: attr,
            imageProcessor: new DefaultImageProcessor(),
            storage: mockStorage,
            temporaryFile: ref fileName,
            validator: new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance));

        using var scaledImage = Image.Load(mockFileSystem.GetFile(mockFileSystem.AllFiles.Single()).Contents);

        Assert.Equal(10, scaledImage.Width);
        Assert.Equal(10, scaledImage.Height);

        Assert.Equal(2, originalImage.Width);
        Assert.Equal(2, originalImage.Height);
    }

    [Fact]
    public void ScalesImage_WithSpecifiedBackgroundColor_WhenItsNotNull()
    {
        var mockStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockStorage.MockFileSystem;
        var fileName = "upload/test.jpg";

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
    
        FileUploadBehavior.CheckUploadedImageAndCreateThumbs(uploadOptions: attr,
            imageProcessor: new DefaultImageProcessor(),
            storage: mockStorage,
            temporaryFile: ref fileName,
            validator: new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance));
    
        using var scaledImage = Image.Load<Rgb24>(mockFileSystem.GetFile(mockFileSystem.AllFiles.Single()).Contents);

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
        var mockStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockStorage.MockFileSystem;
        var fileName = "upload/test.jpg";

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
    
        FileUploadBehavior.CheckUploadedImageAndCreateThumbs(uploadOptions: attr,
            imageProcessor: new DefaultImageProcessor(),
            storage: mockStorage,
            temporaryFile: ref fileName,
            validator: new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance));
    
        using var scaledImage = (Image<Rgb24>)Image.Load(mockFileSystem.GetFile(mockFileSystem.AllFiles.Single()).Contents);

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
        var mockStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockStorage.MockFileSystem;
        var fileName = "upload/test.jpg";

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
    
        Assert.Throws<ArgumentOutOfRangeException>(() => FileUploadBehavior.CheckUploadedImageAndCreateThumbs(uploadOptions: attr,
            imageProcessor: new DefaultImageProcessor(),
            storage: mockStorage,
            temporaryFile: ref fileName,
            validator: new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance)));
    }

    [Fact]
    public void CreatesThumbnails_WhenThumbSizesOnAttr_IsNotNull()
    {
        var mockStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockStorage.MockFileSystem;
        var fileName = "upload/test.jpg";

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
    
        FileUploadBehavior.CheckUploadedImageAndCreateThumbs(uploadOptions: attr,
            imageProcessor: new DefaultImageProcessor(),
            storage: mockStorage,
            temporaryFile: ref fileName,
            validator: new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance));

        Assert.Single(mockFileSystem.AllFiles.Except(new[] {originalFile}));
    }
    
    [Fact]
    public void SetsThumbnailNames_ToTheirRespectiveWidthAndHeight()
    {
        var mockStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockStorage.MockFileSystem;
        var fileName = "upload/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            ThumbQuality = 100,
            ThumbMode = ImageScaleMode.PreserveRatioWithFill,
            ThumbSizes = "100x100;200x200"
        };

        mockFileSystem.AddFile(fileName, new MockFileData(CreateImage(1, 1)));
        var originalFile = mockFileSystem.AllFiles.Single();
        
    
        FileUploadBehavior.CheckUploadedImageAndCreateThumbs(uploadOptions: attr,
            imageProcessor: new DefaultImageProcessor(),
            storage: mockStorage,
            temporaryFile: ref fileName,
            validator: new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance));

        var originalFileNameWithoutExt = mockFileSystem.GetFileNameWithoutExtension(originalFile);

        Assert.Collection(mockFileSystem.AllFiles.Except(new[] {originalFile}).Select(mockFileSystem.GetFileName).OrderBy(fn => fn),
            x => Assert.Equal($"{originalFileNameWithoutExt}_t100x100.jpg", x),
            x => Assert.Equal($"{originalFileNameWithoutExt}_t200x200.jpg", x));
    }
    
    [Fact]
    public void CreatesThumbnails_WithThumbSizesOnAttr()
    {
        var mockStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockStorage.MockFileSystem;
        var fileName = "upload/test.jpg";

        var attr = new ImageUploadEditorAttribute
        {
            ThumbQuality = 100,
            ThumbMode = ImageScaleMode.PreserveRatioWithFill,
            ThumbSizes = "100x100,200x200,300x300"
        };

        mockFileSystem.AddFile(fileName, new MockFileData(CreateImage(1, 1)));
        var originalFile = mockFileSystem.AllFiles.Single();
        using var originalImage = Image.Load(mockFileSystem.GetFile(originalFile).Contents);
    
        FileUploadBehavior.CheckUploadedImageAndCreateThumbs(uploadOptions: attr,
            imageProcessor: new DefaultImageProcessor(),
            storage: mockStorage,
            temporaryFile: ref fileName,
            validator: new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance));


        Assert.Collection(mockFileSystem.AllFiles.Except(new[] {originalFile}).OrderBy(fn => fn),
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
        var mockStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockStorage.MockFileSystem;
        var fileName = "upload/test.jpg";

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
    
        FileUploadBehavior.CheckUploadedImageAndCreateThumbs(uploadOptions: attr,
            imageProcessor: new DefaultImageProcessor(),
            storage: mockStorage,
            temporaryFile: ref fileName,
            validator: new DefaultUploadValidator(new DefaultImageProcessor(), NullTextLocalizer.Instance));

        var white = new Rgb24(255, 255, 255);
        var black = new Rgb24(0, 0, 0);
        
        Assert.Collection(mockFileSystem.AllFiles.Except(new[] {originalFile}).OrderBy(fn => fn),
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