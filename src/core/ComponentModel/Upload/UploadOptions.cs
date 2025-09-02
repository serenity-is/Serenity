using Serenity.Web;

namespace Serenity.ComponentModel;

/// <summary>
/// Set of default upload options
/// </summary>
public class UploadOptions : IUploadFileConstraints, IUploadFileOptions, IUploadImageContrains, IUploadImageOptions
{
    /// <inheritdoc/>
    public bool AllowNonImage { get; set; } = true;
    
    /// <inheritdoc/>
    public int MaxSize { get; set; }

    /// <inheritdoc/>
    public int MinSize { get; set; }

    /// <inheritdoc/>
    public int MaxHeight { get; set; }

    /// <inheritdoc/>
    public int MaxWidth { get; set; }

    /// <inheritdoc/>
    public int MinHeight { get; set; }

    /// <inheritdoc/>
    public int MinWidth { get; set; }

    /// <inheritdoc/>
    public int ScaleQuality { get; set; } = 80;

    /// <inheritdoc/>
    public int ScaleWidth { get; set; }

    /// <inheritdoc/>
    public int ScaleHeight { get; set; }

    /// <inheritdoc/>
    public bool ScaleSmaller { get; set; }

    /// <summary>
    /// Default scale mode
    /// </summary>
    public const ImageScaleMode DefaultScaleMode = ImageScaleMode.PreserveRatioNoFill;

    /// <inheritdoc/>
    public ImageScaleMode ScaleMode { get; set; } = DefaultScaleMode;

    /// <inheritdoc/>
    public string? ScaleBackColor { get; set; }

    /// <inheritdoc/>
    public int ThumbHeight { get; set; }

    /// <inheritdoc/>
    public int ThumbWidth { get; set; }

    /// <inheritdoc/>
    public string? ThumbSizes { get; set; }

    /// <summary>
    /// Default thumb mode
    /// </summary>
    public const ImageScaleMode DefaultThumbMode = ImageScaleMode.PreserveRatioNoFill;

    /// <inheritdoc/>
    public ImageScaleMode ThumbMode { get; set; } = DefaultThumbMode;

    /// <summary>
    /// Default thumb quality
    /// </summary>
    public const int DefaultThumbQuality = 80;

    /// <inheritdoc/>
    public int ThumbQuality { get; set; } = DefaultThumbQuality;

    /// <inheritdoc/>
    public string? ThumbBackColor { get; set; }

    /// <inheritdoc/>
    public bool JsonEncodeValue { get; set; }

    /// <inheritdoc/>
    public string? OriginalNameProperty { get; set; }

    /// <inheritdoc/>
    public bool DisplayFileName { get; set; }
    /// <inheritdoc/>
    public bool CopyToHistory { get; set; }

    /// <inheritdoc/>
    public string? FilenameFormat { get; set; }

    /// <inheritdoc/>
    public string? AllowedExtensions { get; set; }

    /// Default list of image extensions: ".gif;.jpg;.jpeg;.png;"
    public static string DefaultImageExtensions =
        ".gif;.jpg;.jpeg;.png;";

    /// <inheritdoc/>
    public string? ImageExtensions { get; set; } = DefaultImageExtensions;

    /// <inheritdoc/>
    public bool IgnoreExtensionMismatch { get; set; } = true;

    /// <inheritdoc/>
    public bool IgnoreEmptyImage { get; set; }

    /// <inheritdoc/>
    public bool IgnoreInvalidImage { get; set; }
}