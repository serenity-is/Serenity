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

    /// <summary>
    /// If you don't want FileUploadBehavior / MultipleFileUploadBehavior to process this upload, 
    /// and want to handle it manually, set to true (not recommended)
    /// </summary>
    public bool DisableDefaultBehavior { get; }

    /// <summary>
    /// This is a property used to match uploaded files with their origins.
    /// If not specified, will be calculated as: "FullTypeName,AssemblyName:PropertyName"
    /// from the attribute this property is placed on. This way the temporary
    /// upload processor can locate the original attribute by its type name
    /// and validate its settings or generate the expected thumbnail types.
    /// </summary>
    public string? UploadIntent { get; }
}