namespace Serenity.ComponentModel;

/// <summary>
/// Sets formatting type to "BigInt"
/// </summary>
/// <seealso cref="CustomFormatterAttribute" />
public class BigIntFormatterAttribute : CustomFormatterAttribute
{
    /// <summary>
    /// Formatter type key
    /// </summary>
    public const string Key = "BigInt";

    /// <summary>
    /// Initializes a new instance of the <see cref="BigIntFormatterAttribute"/> class.
    /// </summary>
    public BigIntFormatterAttribute()
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
