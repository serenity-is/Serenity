namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "Time" editor.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class TimeEditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "Time";

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeEditorAttribute"/> class.
    /// </summary>
    public TimeEditorAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether empty option should be shown.
    /// </summary>
    /// <value>
    ///   <c>true</c> if no empty option; otherwise, <c>false</c>.
    /// </value>
    public bool NoEmptyOption
    {
        get { return GetOption<bool>("noEmptyOption"); }
        set { SetOption("noEmptyOption", value); }
    }

    /// <summary>
    /// Gets or sets the start hour between 0 and 23.
    /// </summary>
    /// <value>
    /// The start hour.
    /// </value>
    public int StartHour
    {
        get { return GetOption<int>("startHour"); }
        set { SetOption("startHour", value); }
    }

    /// <summary>
    /// Gets or sets the end hour between 0 and 23.
    /// </summary>
    /// <value>
    /// The end hour.
    /// </value>
    public int EndHour
    {
        get { return GetOption<int>("endHour"); }
        set { SetOption("endHour", value); }
    }

    /// <summary>
    /// Gets or sets the interval minutes.
    /// </summary>
    /// <value>
    /// The interval minutes.
    /// </value>
    public int IntervalMinutes
    {
        get { return GetOption<int>("intervalMinutes"); }
        set { SetOption("intervalMinutes", value); }
    }
}