namespace Serenity.ComponentModel;

/// <summary>
/// Sets formatting type to "Number"
/// </summary>
/// <seealso cref="CustomFormatterAttribute" />
public class NumberFormatterAttribute : CustomFormatterAttribute
{
    /// <summary>
    /// Formatter type key
    /// </summary>
    public const string Key = "Number";

    /// <summary>
    /// Initializes a new instance of the <see cref="NumberFormatterAttribute"/> class.
    /// </summary>
    public NumberFormatterAttribute()
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
