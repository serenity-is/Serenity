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
    /// Gets or sets the icon corresponding to falsy value.
    /// </summary>
    /// <value>
    /// The false icon.
    /// </value>
    public string? FalseIcon
    {
        get { return GetOption<string>("falseIcon"); }
        set { SetOption("falseIcon", value); }
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
    /// Gets or sets the icon corresponding to truthy value.
    /// </summary>
    /// <value>
    /// The true icon.
    /// </value>
    public string? TrueIcon
    {
        get { return GetOption<string>("trueIcon"); }
        set { SetOption("trueIcon", value); }
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

    /// <summary>
    /// Gets or sets a value indicating whether to show text next to checkbox icon.
    /// Defaults to true
    /// </summary>
    public bool? ShowText
    {
        get { return GetOption<bool?>("showText"); }
        set { SetOption("showText", value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show hint (text) on hover when
    /// text is not displayed next to checkbox icon.
    /// When displayed in header filter, defaults to false, otherwise defaults to true
    /// when showText is false.
    /// </summary>
    public bool? ShowHint
    {
        get { return GetOption<bool?>("showHint"); }
        set { SetOption("showHint", value); }
    }
}

