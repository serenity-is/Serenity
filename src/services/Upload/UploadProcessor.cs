using System.IO;

namespace Serenity.Web;

/// <summary>
/// Obsolete class for upload processing.
/// </summary>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="storage">Upload storage</param>
/// <param name="_">Exception logger, not used.</param>
/// <exception cref="ArgumentNullException"><paramref name="storage"/> is <c>null</c>.</exception>
[Obsolete("Please inject and use IUploadProcessor interface")]
#pragma warning disable CS9113 // Parameter is unread.
public class UploadProcessor(IUploadStorage storage, IExceptionLogger _ = null) : ProcessedUploadInfo
#pragma warning restore CS9113 // Parameter is unread.
{
    private readonly IUploadStorage storage = storage ?? throw new ArgumentNullException(nameof(storage));

    /// <summary>
    /// Gets or sets the thumb width.
    /// </summary>
    public int ThumbWidth { get; set; }

    /// <summary>
    /// Gets or sets the thumb height.
    /// </summary>
    public int ThumbHeight { get; set; }

    /// <summary>
    /// Gets or sets the thumb back color.
    /// </summary>
    public string ThumbBackColor { get; set; } = null;

    /// <summary>
    /// Gets or sets the thumb scale mode.
    /// </summary>
    public ImageScaleMode ThumbScaleMode { get; set; }

    /// <summary>
    /// Gets or sets the thumb quality.
    /// </summary>
    public int ThumbQuality { get; set; }

    /// <summary>
    /// Processes an upload.
    /// </summary>
    /// <param name="fileContent">File content</param>
    /// <param name="extension">File extension</param>
    /// <param name="localizer">Text localizer</param>
    /// <param name="options">Upload options</param>
    /// <returns><c>true</c> if the upload was processed successfully.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fileContent"/> is <c>null</c>.</exception>
    public bool ProcessStream(Stream fileContent, string extension, 
        ITextLocalizer localizer, IUploadOptions options = null)
    {
        ArgumentNullException.ThrowIfNull(fileContent);

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
        FileSize = result.FileSize;
        ImageHeight = result.ImageWidth;
        ImageHeight = result.ImageHeight;
        IsImage = result.IsImage;
        TemporaryFile = result.TemporaryFile;
        return true;
    }
}
