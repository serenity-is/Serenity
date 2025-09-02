namespace Serenity.ComponentModel;

/// <summary>
/// Sets formatting type to "Date"
/// </summary>
/// <seealso cref="CustomFormatterAttribute" />
public class DateFormatterAttribute : CustomFormatterAttribute
{
    /// <summary>
    /// Formatter type key
    /// </summary>
    public const string Key = "Date";

    /// <summary>
    /// Initializes a new instance of the <see cref="DateFormatterAttribute"/> class.
    /// </summary>
    public DateFormatterAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Gets or sets the display format.
    /// </summary>
    /// <value>
    /// The display format.
    /// </value>
    public string? DisplayFormat
    {
        get { return GetOption<string>("displayFormat"); }
        set { SetOption("displayFormat", value); }
    }
}