namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "HtmlReportContent" editor.
/// This is generally a CK editor with only functionality compatible with common
/// reporting tools like SSRS, Telerik, DevExpress etc. enabled.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class HtmlReportContentEditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "HtmlReportContent";

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlReportContentEditorAttribute"/> class.
    /// </summary>
    public HtmlReportContentEditorAttribute()
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