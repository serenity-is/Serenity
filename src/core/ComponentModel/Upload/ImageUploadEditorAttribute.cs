namespace Serenity.ComponentModel;

/// <summary>
/// Sets editor type as "ImageUpload", which only allows image files.
/// Make sure you use this attribute in Row.cs, not Form.cs as 
/// the image upload behavior only works if it is in row, otherwise
/// your files will stay in temporary directory.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class ImageUploadEditorAttribute : BaseUploadEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "ImageUpload";

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageUploadEditorAttribute"/> class.
    /// </summary>
    public ImageUploadEditorAttribute()
        : base(Key)
    {
        AllowNonImage = false;
    }

    /// <inheritdoc/>
    public override bool IsMultiple => false;
}