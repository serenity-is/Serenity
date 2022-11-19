namespace Serenity.ComponentModel;

/// <summary>
/// Contstraints for the uploaded image.
/// </summary>
public interface IUploadImageContrains
{
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