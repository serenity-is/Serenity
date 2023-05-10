namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "HtmlContent" editor.
/// This is generally a CK editor that contains more functionalities
/// compared to other ones.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class HtmlContentEditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "HtmlContent";

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlContentEditorAttribute"/> class.
    /// </summary>
    public HtmlContentEditorAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Gets or sets the cols for underlying textarea.
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
    /// Gets or sets the rows for underlying textarea.
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