namespace Serenity.ComponentModel;

/// <summary>
/// Runs FileUploadBehavior or MultipleFileUploadBehavior behaviors.
/// Make sure you use this attribute in Row.cs, not Form.cs as 
/// the image upload behavior only works if it is in row, otherwise
/// your files will stay in temporary directory.
/// </summary>
public interface IUploadEditor
{
    /// <summary>
    /// Sets if the editor is going to be used for multiple file upload.
    /// </summary>
    public bool IsMultiple { get; }
}