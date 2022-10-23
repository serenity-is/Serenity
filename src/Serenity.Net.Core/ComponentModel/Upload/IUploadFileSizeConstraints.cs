namespace Serenity.ComponentModel;

/// <summary>
/// Constraints of the uploaded file size.
/// </summary>
public interface IUploadFileSizeConstraints
{
    /// <summary>
    /// Maximum size in bytes of the uploaded file.
    /// </summary>
    public int MaxSize { get; }
    
    /// <summary>
    /// Minimum size in bytes of the uploaded file.
    /// </summary>
    public int MinSize { get; }
}