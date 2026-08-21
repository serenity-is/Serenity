namespace Serenity;

/// <summary>
/// Culture options that are passed to the client side.
/// </summary>
public class ScriptCulture
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptCulture"/> class
    /// using <see cref="CultureInfo.CurrentCulture"/>.
    /// </summary>
    public ScriptCulture()
        : this(CultureInfo.CurrentCulture)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptCulture"/> class.
    /// </summary>
    /// <param name="culture">The culture to read settings from.</param>
    public ScriptCulture(CultureInfo culture)
    {
        var order = DateHelper.DateElementOrderFor(culture.DateTimeFormat.ShortDatePattern);
        DateOrder = DateHelper.DateOrderString(order);
        DateFormat = DateHelper.DefaultDateFormat(order);
        DateTimeFormat = DateHelper.DefaultDateTimeFormat(order);
        DateSeparator = string.IsNullOrEmpty(culture.DateTimeFormat.DateSeparator) ?
            DateTime.Now.ToString("yy/MM/dd", culture.DateTimeFormat)[2].ToString() :
            culture.DateTimeFormat.DateSeparator;
        DecimalSeparator = culture.NumberFormat.NumberDecimalSeparator;
        GroupSeparator = culture.NumberFormat.NumberGroupSeparator;
    }

    /// <summary>
    /// Gets or sets the date year-month-day ordering, e.g. <c>d/M/y</c>.
    /// </summary>
    public string DateOrder { get; set; }

    /// <summary>
    /// Gets or sets the date format.
    /// </summary>
    public string DateFormat { get; set; }

    /// <summary>
    /// Gets or sets the date separator.
    /// </summary>
    public string DateSeparator { get; set; }

    /// <summary>
    /// Gets or sets the date/time format.
    /// </summary>
    public string DateTimeFormat { get; set; }

    /// <summary>
    /// Gets or sets the decimal separator.
    /// </summary>
    public string DecimalSeparator { get; set; }

    /// <summary>
    /// Gets or sets the group separator.
    /// </summary>
    public string GroupSeparator { get; set; }
}