namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "Boolean" editor.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class BooleanEditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "Boolean";

    /// <summary>
    /// Initializes a new instance of the <see cref="BooleanEditorAttribute"/> class.
    /// </summary>
    public BooleanEditorAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether the checkbox is placed first
    /// and the field caption is shown after it, on the end side (right in LTR), instead of the default
    /// caption-left layout. Useful for fitting several checkboxes in one row.
    /// </summary>
    public bool LabelOnEnd
    {
        get { return GetOption<bool>("labelOnEnd"); }
        set { SetOption("labelOnEnd", value); }
    }
}