namespace Serenity.ComponentModel;

/// <summary>
/// Constraints of the uploaded file size.
/// </summary>
public interface IUploadFileConstraints : IUploadOptions
{
    /// <summary>
    /// Should non-image uploads be allowed.
    /// </summary>
    public bool AllowNonImage { get; }

    /// <summary>
    /// Semicolon separated list of allowed file extensions, 
    /// like ".xlsx;.docx;.jpg" etc.
    /// If specified, only allow files with these extensions.
    /// This is currently only used by the upload behavior,
    /// the editor and temporary upload does not yet use this setting.
    /// </summary>
    public string? AllowedExtensions { get; }

    /// <summary>
    /// Semicolon separated list of image extensions. The default list is 
    /// ".gif;.jpg;.jpeg;.png;"
    /// If specified, only these set of file extensions are considered
    /// as images and thumbnails are only generated for them.
    /// This is currently only used at behavior level. The upload editors and
    /// temporary upload do not check them yet.
    /// Note that if AllowNonImage is false, only these extensions are
    /// allowed and only if they contain a valid image.
    /// </summary>
    public string? ImageExtensions { get; }

    /// <summary>
    /// Maximum size in bytes of the uploaded file.
    /// </summary>
    public int MaxSize { get; }
    
    /// <summary>
    /// Minimum size in bytes of the uploaded file.
    /// </summary>
    public int MinSize { get; }
}