namespace Serenity.ComponentModel;

/// <summary>
/// Sets formatting type to "Boolean"
/// </summary>
/// <seealso cref="CustomFormatterAttribute" />
public class BooleanFormatterAttribute : CustomFormatterAttribute
{
    /// <summary>
    /// Formatter type key
    /// </summary>
    public const string Key = "Boolean";

    /// <summary>
    /// Initializes a new instance of the <see cref="BooleanFormatterAttribute"/> class.
    /// </summary>
    public BooleanFormatterAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Gets or sets the text corresponding to falsy value.
    /// </summary>
    /// <value>
    /// The false text.
    /// </value>
    public string? FalseText
    {
        get { return GetOption<string>("falseText"); }
        set { SetOption("falseText", value); }
    }

    /// <summary>
    /// Gets or sets the text corresponding to truthy value.
    /// </summary>
    /// <value>
    /// The true text.
    /// </value>
    public string? TrueText
    {
        get { return GetOption<string>("trueText"); }
        set { SetOption("trueText", value); }
    }
}

