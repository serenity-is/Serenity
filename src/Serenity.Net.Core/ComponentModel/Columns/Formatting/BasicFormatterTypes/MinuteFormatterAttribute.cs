namespace Serenity.ComponentModel;

/// <summary>
/// Sets formatting type to "Minute" which formats an integer value in HH:mm format.
/// </summary>
/// <seealso cref="CustomFormatterAttribute" />
public class MinuteFormatterAttribute : CustomFormatterAttribute
{
    /// <summary>
    /// Formatter type key
    /// </summary>
    public const string Key = "Serenity.MinuteFormatter";

    /// <summary>
    /// Initializes a new instance of the <see cref="MinuteFormatterAttribute"/> class.
    /// </summary>
    public MinuteFormatterAttribute()
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
