namespace Serenity.ComponentModel;

/// <summary>
/// Sets editor type to "MultipleImageUpload" which doesn't allow
/// non-image file types by default.
/// </summary>
/// <seealso cref="ImageUploadEditorAttribute" />
public class MultipleImageUploadEditorAttribute : BaseUploadEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "MultipleImageUpload";

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleImageUploadEditorAttribute"/> class.
    /// </summary>
    public MultipleImageUploadEditorAttribute()
        : base(Key)
    {
        AllowNonImage = false;
        JsonEncodeValue = true;
    }

    /// <inheritdoc />
    public override bool IsMultiple => true;
}