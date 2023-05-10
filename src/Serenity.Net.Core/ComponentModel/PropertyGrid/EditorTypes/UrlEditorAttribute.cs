namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "Url" editor.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class URLEditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "URL";

    /// <summary>
    /// Initializes a new instance of the <see cref="URLEditorAttribute"/> class.
    /// </summary>
    public URLEditorAttribute()
        : base(Key)
    {
    }
}