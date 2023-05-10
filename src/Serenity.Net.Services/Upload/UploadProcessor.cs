using System.IO;

namespace Serenity.Web;

/// <summary>
/// Obsolete class for upload processing
/// </summary>
[Obsolete("Please inject and use IUploadProcessor interface")]
public class UploadProcessor : ProcessedUploadInfo
{
    private readonly IUploadStorage storage;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="storage">Upload storage</param>
    /// <param name="_">Exception logger, not used.</param>
    /// <exception cref="ArgumentNullException">Storage is null</exception>
    public UploadProcessor(IUploadStorage storage, IExceptionLogger _ = null)
    {
        ThumbBackColor = null;
        this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    /// <summary>
    /// Thumb width
    /// </summary>
    public int ThumbWidth { get; set; }

    /// <summary>
    /// Thumb height
    /// </summary>
    public int ThumbHeight { get; set; }

    /// <summary>
    /// Thumb back color
    /// </summary>
    public string ThumbBackColor { get; set; }

    /// <summary>
    /// Thumb scale mode
    /// </summary>
    public ImageScaleMode ThumbScaleMode { get; set; }

    /// <summary>
    /// Thumb quality
    /// </summary>
    public int ThumbQuality { get; set; }

    /// <summary>
    /// Processes an upload
    /// </summary>
    /// <param name="fileContent">File content</param>
    /// <param name="extension">File extension</param>
    /// <param name="localizer">Text localizer</param>
    /// <param name="options">Upload options</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool ProcessStream(Stream fileContent, string extension, 
        ITextLocalizer localizer, IUploadOptions options = null)
    {
        if (fileContent is null)
            throw new ArgumentNullException(nameof(fileContent));

        options ??= new UploadOptions
        {
            ThumbBackColor = ThumbBackColor,
            ThumbHeight = ThumbHeight,
            ThumbWidth = ThumbWidth,
            ThumbQuality = ThumbQuality,
            ThumbMode = ThumbScaleMode
        };

        var imageProcessor = new DefaultImageProcessor();
        var uploadProcessor = new DefaultUploadProcessor(new DefaultImageProcessor(),
            storage, new DefaultUploadValidator(imageProcessor, localizer));

        var result = uploadProcessor.Process(fileContent, "___tempfile__" + extension, options);
        ErrorMessage = result.ErrorMessage;
        FileSize = result.FileSize;
        ImageHeight = result.ImageWidth;
        ImageHeight = result.ImageHeight;
        IsImage = result.IsImage;
        Success = result.Success;
        TemporaryFile = result.TemporaryFile;
        return result.Success;
    }
}
