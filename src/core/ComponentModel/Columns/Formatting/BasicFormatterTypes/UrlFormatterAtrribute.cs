namespace Serenity.ComponentModel;

/// <summary>
/// Sets formatting type to "Url"
/// </summary>
/// <seealso cref="CustomFormatterAttribute" />
public class UrlFormatterAttribute : CustomFormatterAttribute
{
    /// <summary>
    /// Formatter type key
    /// </summary>
    public const string Key = "Url";

    /// <summary>
    /// Initializes a new instance of the <see cref="UrlFormatterAttribute"/> class.
    /// </summary>
    public UrlFormatterAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Gets or sets the display property to show in hyperlink as text.
    /// </summary>
    /// <value>
    /// The display property.
    /// </value>
    public string? DisplayProperty
    {
        get { return GetOption<string>("displayProperty"); }
        set { SetOption("displayProperty", value); }
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

    /// <summary>
    /// Gets or sets the URL property.
    /// </summary>
    /// <value>
    /// The URL property.
    /// </value>
    public string? UrlProperty
    {
        get { return GetOption<string>("urlProperty"); }
        set { SetOption("urlProperty", value); }
    }

    /// <summary>
    /// Gets or sets the URL format.
    /// </summary>
    /// <value>
    /// The URL format.
    /// </value>
    public string? UrlFormat
    {
        get { return GetOption<string>("urlFormat"); }
        set { SetOption("urlFormat", value); }
    }

    /// <summary>
    /// Gets or sets the target window, e.g. "_blank".
    /// </summary>
    /// <value>
    /// The target.
    /// </value>
    public string? Target
    {
        get { return GetOption<string>("target"); }
        set { SetOption("target", value); }
    }
}
