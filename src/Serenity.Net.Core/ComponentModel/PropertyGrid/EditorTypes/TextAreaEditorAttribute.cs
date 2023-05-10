namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "TextArea" editor.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class TextAreaEditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "TextArea";

    /// <summary>
    /// Initializes a new instance of the <see cref="TextAreaEditorAttribute"/> class.
    /// </summary>
    public TextAreaEditorAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Gets or sets the cols for textarea.
    /// </summary>
    /// <value>
    /// The cols.
    /// </value>
    public int Cols
    {
        get { return GetOption<int>("cols"); }
        set { SetOption("cols", value); }
    }

    /// <summary>
    /// Gets or sets the rows for textarea.
    /// </summary>
    /// <value>
    /// The rows.
    /// </value>
    public int Rows
    {
        get { return GetOption<int>("rows"); }
        set { SetOption("rows", value); }
    }
}