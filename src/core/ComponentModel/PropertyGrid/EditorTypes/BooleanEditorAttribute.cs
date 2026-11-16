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
    /// Gets or sets where the caption of the field is, at the start in the caption area by default, or after the checkbox,
    /// which is useful for fitting several checkboxes in one row, see <see cref="CheckLabelPosition"/>.
    /// </summary>
    public CheckLabelPosition LabelPosition
    {
        get { return GetOption<CheckLabelPosition>("labelPosition"); }
        set { SetOption("labelPosition", value); }
    }
}