namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "DateYear" editor.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class DateYearEditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "DateYear";

    /// <summary>
    /// Initializes a new instance of the <see cref="DateYearEditorAttribute"/> class.
    /// </summary>
    public DateYearEditorAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Gets or sets the maximum year.
    /// It can be written as an integer value, or +50, -20 etc. 
    /// corresponding to current year plus or minus years.
    /// </summary>
    /// <value>
    /// The maximum year.
    /// </value>
    public string? MaxYear
    {
        get { return GetOption<string>("maxYear"); }
        set { SetOption("maxYear", value); }
    }

    /// <summary>
    /// Gets or sets the minimum year.
    /// It can be written as an integer value, or +50, -20 etc. 
    /// corresponding to current year plus or minus years.
    /// </summary>
    /// <value>
    /// The minimum year. 
    /// </value>
    public string? MinYear
    {
        get { return GetOption<string>("minYear"); }
        set { SetOption("minYear", value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the years should be listed in descending order.
    /// </summary>
    /// <value>
    ///   <c>true</c> if descending; otherwise, <c>false</c>.
    /// </value>
    public bool Descending
    {
        get { return GetOption<bool>("descending"); }
        set { SetOption("descending", value); }
    }
}