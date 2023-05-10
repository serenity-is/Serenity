namespace Serenity.ComponentModel;

/// <summary>
/// Sets editor type as "ImageUpload" (single), while allowing non-image files.
/// </summary>
/// <seealso cref="ImageUploadEditorAttribute" />
public class FileUploadEditorAttribute : BaseUploadEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "ImageUpload";

    /// <summary>
    /// Initializes a new instance of the <see cref="FileUploadEditorAttribute"/> class.
    /// </summary>
    public FileUploadEditorAttribute()
        : base(Key)
    {
        AllowNonImage = true;
    }

    /// <inheritdoc/>
    public override bool IsMultiple => false;
}