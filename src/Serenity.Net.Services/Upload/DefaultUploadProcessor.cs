using Microsoft.Extensions.Logging;

namespace Serenity.Web;

/// <summary>
/// Default implementation for <see cref="IUploadProcessor"/>
/// </summary>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="imageProcessor">Image processor</param>
/// <param name="uploadStorage">Upload storage</param>
/// <param name="uploadValidator">Upload validator</param>
/// <param name="logger">Logger</param>
/// <param name="avScanner">Optional antivirus scanner</param>
/// <exception cref="ArgumentNullException"></exception>
public class DefaultUploadProcessor(IImageProcessor imageProcessor, IUploadStorage uploadStorage, IUploadValidator uploadValidator,
    ILogger<DefaultUploadProcessor> logger = null,
    IUploadAVScanner avScanner = null) : IUploadProcessor
{
    /// <summary>
    /// Image processor
    /// </summary>
    protected readonly IImageProcessor imageProcessor = imageProcessor ?? throw new ArgumentNullException(nameof(imageProcessor));

    /// <summary>
    /// Upload storage
    /// </summary>
    protected readonly IUploadStorage uploadStorage = uploadStorage ?? throw new ArgumentNullException(nameof(uploadStorage));

    /// <summary>
    /// Upload validator
    /// </summary>
    protected readonly IUploadValidator uploadValidator = uploadValidator ?? throw new ArgumentNullException(nameof(uploadValidator));

    /// <summary>
    /// Logger
    /// </summary>
    protected readonly ILogger<DefaultUploadProcessor> logger = logger;

    /// <summary>
    /// AV Scanner
    /// </summary>
    protected readonly IUploadAVScanner avScanner = avScanner;

    /// <inheritdoc/>
    public virtual ProcessedUploadInfo Process(System.IO.Stream stream, string filename, IUploadOptions options)
    {
        if (stream is null)
            throw new ArgumentNullException(nameof(stream));

        if (filename is null)
            throw new ArgumentNullException(nameof(filename));

        if (options is null)
            throw new ArgumentNullException(nameof(options));

        UploadPathHelper.CheckFileNameSecurity(filename);

        bool isExistingTemporary = filename.StartsWith("temporary/", StringComparison.Ordinal) &&
            uploadStorage.FileExists(filename);

        if (!isExistingTemporary)
            avScanner?.Scan(stream, filename);

        var result = new ProcessedUploadInfo
        {
            FileSize = stream.Length
        };

        var success = false;
        try
        {
            try
            {
                uploadValidator.ValidateFile(options as IUploadFileConstraints ?? new UploadOptions(),
                    stream, filename, out bool isImageExtension);

                object image = null;
                if (isImageExtension)
                    uploadValidator.ValidateImage(options as IUploadImageContrains ?? new UploadOptions(),
                        stream, filename, out image);
                try
                {
                    uploadStorage.PurgeTemporaryFiles();

                    if (!isExistingTemporary)
                    {
                        var basePath = "temporary/" + Guid.NewGuid().ToString("N");
                        stream.Seek(0, System.IO.SeekOrigin.Begin);
                        result.TemporaryFile = uploadStorage.WriteFile(basePath + System.IO.Path.GetExtension(filename),
                            stream, OverwriteOption.Disallowed);
                        uploadStorage.SetOriginalName(result.TemporaryFile, System.IO.Path.GetFileName(filename));
                    }
                    else
                    {
                        result.TemporaryFile = filename;
                    }

                    result.IsImage = isImageExtension && image != null;
                    if (result.IsImage)
                    {
                        stream.Close();
                        result.TemporaryFile = ProcessImage(image, options as IUploadImageOptions ?? new UploadOptions(), result.TemporaryFile);
                    }
                    success = true;
                }
                finally
                {
                    (image as IDisposable)?.Dispose();
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error occurred while processing upload!");
                throw;
            }
        }
        finally
        {
            if (!success && !string.IsNullOrEmpty(result.TemporaryFile) && !isExistingTemporary)
                uploadStorage.DeleteFile(result.TemporaryFile);

            stream?.Dispose();
        }

        return result;
    }

    /// <summary>
    /// Depending on the image upload options, scales image, creates default and
    /// additional thumbs and saves them to the upload storage files.
    /// </summary>
    /// <param name="image">Input image</param>
    /// <param name="options">Upload image options</param>
    /// <param name="imageFile">Image file path</param>
    /// <returns>Scaled image file path. It might be different then the passed file if scaling is performed.
    /// E.g. the uploaded file might be .png while the scaled image might be .jpg</returns>
    protected virtual string ProcessImage(object image, IUploadImageOptions options, string imageFile)
    {
        imageFile = ScaleMainImage(image, options, imageFile)?.Filename ?? imageFile;
        CreateDefaultThumb(image, options, imageFile);
        CreateAdditionalThumbs(image, options, imageFile);
        return imageFile;
    }

    /// <summary>
    /// Scales the temporary image with provided upload image options if required 
    /// based on the options and saves the result to the target upload storage file
    /// </summary>
    /// <param name="image">Image object</param>
    /// <param name="options">Image upload options</param>
    /// <param name="imageFile">Main image file</param>
    /// <returns>The resulting image file path</returns>
    /// <exception cref="ArgumentNullException">image or options is null</exception>
    protected virtual ScaleImageAsResult ScaleMainImage(object image, IUploadImageOptions options, string imageFile)
    {
        if (image is null)
            throw new ArgumentNullException(nameof(image));

        if (options is null)
            throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrEmpty(imageFile))
            throw new ArgumentNullException(nameof(imageFile));

        var (imageWidth, imageHeight) = imageProcessor.GetImageSize(image);
        var scaleSmaller = options.ScaleSmaller == true;

        ScaleImageAsResult result = null;
        if ((options.ScaleWidth > 0 || options.ScaleHeight > 0) &&
            (options.ScaleWidth != imageWidth || options.ScaleHeight != imageHeight) &&
            ((options.ScaleWidth > 0 && (scaleSmaller || options.ScaleWidth < imageWidth)) ||
             (options.ScaleHeight > 0 && (scaleSmaller || options.ScaleHeight < imageHeight))))
        {
            var baseFile = System.IO.Path.ChangeExtension(imageFile, null);
            var originalName = uploadStorage.GetOriginalName(imageFile);
            result = ScaleImageAs(image, options.ScaleWidth, options.ScaleHeight, options.ScaleMode, 
                options.ScaleBackColor, new ImageEncoderParams { Quality = options.ScaleQuality },
                baseFile + ".jpg");

            imageWidth = result.Width;
            imageHeight = result.Height;

            if (!string.IsNullOrEmpty(originalName))
                uploadStorage.SetOriginalName(result.Filename, System.IO.Path.ChangeExtension(originalName, ".jpg"));
        }

        uploadStorage.SetFileMetadata(result?.Filename ?? imageFile, new Dictionary<string, string>()
        {
            [FileMetadataKeys.ImageSize] = imageWidth.ToInvariant() + "x" + imageHeight.ToInvariant(),
        }, overwriteAll: false);

        return result;
    }

    /// <summary>
    /// Creates the default thumbnail for image if the size is provided
    /// in the upload image options (ThumbWidth and ThumbHeight >= 0) and saves it to the target upload storage file
    /// </summary>
    /// <param name="image">Image</param>
    /// <param name="options">Upload image options</param>
    /// <param name="imageFile">Main image file</param>
    /// <exception cref="ArgumentNullException">image, options or temporaryFile is null</exception>
    public virtual ScaleImageAsResult CreateDefaultThumb(object image, IUploadImageOptions options, string imageFile)
    {
        if (image is null)
            throw new ArgumentNullException(nameof(image));

        if (options is null)
            throw new ArgumentNullException(nameof(options));

        if ((options.ThumbWidth > 0 || options.ThumbHeight > 0) &&
            (options.ThumbWidth >= 0 && options.ThumbHeight >= 0))
        {
            var result = ScaleImageAs(image, options.ThumbWidth, options.ThumbHeight, 
                options.ThumbMode, options.ThumbBackColor, 
                new ImageEncoderParams { Quality = options.ThumbQuality },
                System.IO.Path.ChangeExtension(imageFile, null) + "_t.jpg");

            uploadStorage.SetFileMetadata(imageFile, new Dictionary<string, string>()
            {
                [FileMetadataKeys.ThumbSize] = result.Width.ToInvariant() + "x" + result.Height.ToInvariant()
            }, overwriteAll: false);

            return result;
        }

        return null;
    }

    /// <summary>
    /// Creates additional thumbs if specified in the upload image options,
    /// and saves them to the target upload storage
    /// </summary>
    /// <param name="image">Image</param>
    /// <param name="options">Upload image options</param>
    /// <param name="imageFile">Main image file</param>
    /// <exception cref="ArgumentNullException">image, options or temporaryFile is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">options.ThumbSizes contains invalid values</exception>
    protected virtual IEnumerable<ScaleImageAsResult> CreateAdditionalThumbs(object image, 
        IUploadImageOptions options, string imageFile)
    {
        if (image is null)
            throw new ArgumentNullException(nameof(image));

        if (options is null)
            throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrEmpty(imageFile))
            throw new ArgumentNullException(nameof(imageFile));

        var result = new List<ScaleImageAsResult>();
        var thumbSizes = options.ThumbSizes.TrimToNull();
        if (thumbSizes is null)
            return result;

        var baseFile = System.IO.Path.ChangeExtension(imageFile, null);

        foreach (var sizeStr in thumbSizes.Replace(";", ",", StringComparison.Ordinal).Split([',']))
        {
            var dims = sizeStr.ToUpperInvariant().Split(['X']);
            if (dims.Length != 2 ||
                !int.TryParse(dims[0], out int w) ||
                !int.TryParse(dims[1], out int h) ||
                w < 0 ||
                h < 0 ||
                (w == 0 && h == 0))
                throw new ArgumentOutOfRangeException(nameof(thumbSizes));

            var thumbFile = baseFile + "_t" + w.ToInvariant() + "x" + h.ToInvariant() + ".jpg";

            result.Add(ScaleImageAs(image, w, h, options.ThumbMode, options.ThumbBackColor,
                new ImageEncoderParams { Quality = options.ThumbQuality },
                thumbFile));
        }

        return result;
    }

    /// <summary>
    /// Scales an image and saves it to an upload storage file
    /// </summary>
    /// <param name="image">Source image</param>
    /// <param name="width">Target width</param>
    /// <param name="height">Target height</param>
    /// <param name="mode">Scale mode</param>
    /// <param name="backgroundColor">Pad color</param>
    /// <param name="encoderParams">Encoder parameters for target image</param>
    /// <param name="targetFile">Path</param>
    /// <exception cref="ArgumentNullException">One of inputs is null</exception>
    protected virtual ScaleImageAsResult ScaleImageAs(object image, int width, int height, 
        ImageScaleMode mode, string backgroundColor, ImageEncoderParams encoderParams, string targetFile)
    {
        var scaledImage = imageProcessor.Scale(image, width, height, mode, backgroundColor, inplace: false);
        try
        {
            var size = imageProcessor.GetImageSize(scaledImage);

            using var ms = new System.IO.MemoryStream();
            imageProcessor.Save(scaledImage, ms, "image/jpeg", encoderParams);
            ms.Seek(0, System.IO.SeekOrigin.Begin);
            return new ScaleImageAsResult
            {
                Filename = uploadStorage.WriteFile(targetFile, ms, OverwriteOption.Overwrite),
                Width = size.width,
                Height = size.height
            };
        }
        finally
        {
            (scaledImage as IDisposable)?.Dispose();
        }
    }

    /// <summary>
    /// Result for the scale image as operation
    /// </summary>
    public class ScaleImageAsResult
    {
        /// <summary>
        /// Resulting filename
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Resulting image height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Resulting image width
        /// </summary>
        public int Width { get; set; }
    }
}
