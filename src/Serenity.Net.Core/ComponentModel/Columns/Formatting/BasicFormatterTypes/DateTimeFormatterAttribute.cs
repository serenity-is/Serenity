namespace Serenity.ComponentModel;

/// <summary>
/// Sets formatting type to "DateTime".
/// </summary>
/// <seealso cref="CustomFormatterAttribute" />
public class DateTimeFormatterAttribute : CustomFormatterAttribute
{
    /// <summary>
    /// Formatter type key
    /// </summary>
    public const string Key = "DateTime";

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeFormatterAttribute"/> class.
    /// </summary>
    public DateTimeFormatterAttribute()
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
