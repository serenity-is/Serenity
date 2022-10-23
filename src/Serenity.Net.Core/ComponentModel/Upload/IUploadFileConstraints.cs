namespace Serenity.ComponentModel;

/// <summary>
/// Constraints of the uploaded file.
/// </summary>
public interface IUploadFileConstraints
{
    /// <summary>
    /// Should non-image uploads be allowed.
    /// </summary>
    public bool AllowNonImage { get; }
}