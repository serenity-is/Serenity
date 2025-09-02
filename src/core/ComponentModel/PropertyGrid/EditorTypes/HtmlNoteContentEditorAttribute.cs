namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "HtmlNoteContent" editor.
/// This is generally a CK editor with only basic functionality for notes enabled.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class HtmlNoteContentEditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "HtmlNoteContent";

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlNoteContentEditorAttribute"/> class.
    /// </summary>
    public HtmlNoteContentEditorAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Gets or sets the cols of underlying textarea.
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
    /// Gets or sets the rows of underlying textarea.
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