namespace Serenity.ComponentModel;

/// <summary>
/// Sets filtering type to "Boolean"
/// </summary>
/// <seealso cref="CustomFilteringAttribute" />
public class BooleanFilteringAttribute : CustomFilteringAttribute
{
    /// <summary>
    /// Filtering type key
    /// </summary>
    public const string Key = "Boolean";

    /// <summary>
    /// Initializes a new instance of the <see cref="BooleanFilteringAttribute"/> class.
    /// </summary>
    public BooleanFilteringAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Gets or sets the text used of false value.
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
    /// Gets or sets the text used for true value.
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