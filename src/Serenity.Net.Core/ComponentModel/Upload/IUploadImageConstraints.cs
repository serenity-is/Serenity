namespace Serenity.ComponentModel;

/// <summary>
/// Contstraints for the uploaded image.
/// </summary>
public interface IUploadImageContrains : IUploadOptions
{
    /// <summary>
    /// If set to false, the upload behavior raises an error when it 
    /// detects the extension does not match the actual format of 
    /// the uploaded file. 
    /// For example, if the uploaded file is "test.jpg" but it has a "png"
    /// image inside it raises an error.
    /// Default is true, so such mismatches are ignored
    /// </summary>
    public bool IgnoreExtensionMismatch { get; }

    /// <summary>
    /// If the file contains an empty image, 
    /// ignore it instead of raising an error.
    /// </summary>
    public bool IgnoreEmptyImage { get; }

    /// <summary>
    /// If the file contains an invalid image, 
    /// ignore it instead of raising an error.
    /// </summary>
    public bool IgnoreInvalidImage { get; }

    /// <summary>
    /// Maximum height in pixels of the uploaded image.
    /// </summary>
    public int MaxHeight { get; }

    /// <summary>
    /// Maximum width in pixels of the uploaded image.
    /// </summary>
    public int MaxWidth { get; }

    /// <summary>
    /// Minimum height in pixels of the uploaded image.
    /// </summary>
    public int MinHeight { get; }

    /// <summary>
    /// Minimum width in pixels of the uploaded image.
    /// </summary>
    public int MinWidth { get; }
}