namespace Serenity;

/// <summary>
/// Culture options that are passed to the client side
/// </summary>
public class ScriptCulture
{
    /// <summary>
    /// Creates a new instance of the class using <see cref="CultureInfo.CurrentCulture"/>
    /// </summary>
    public ScriptCulture()
        : this(CultureInfo.CurrentCulture)
    {
    }

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="culture">The culture to read settings from</param>
    public ScriptCulture(CultureInfo culture)
    {
        var order = DateHelper.DateElementOrderFor(culture.DateTimeFormat.ShortDatePattern);
        DateOrder = DateHelper.DateOrderString(order);
        DateFormat = DateHelper.DefaultDateFormat(order);
        DateTimeFormat = DateHelper.DefaultDateTimeFormat(order);
        DateSeparator = DateTime.MaxValue.ToString("yy/MM/dd", culture.DateTimeFormat)[2].ToString();
        DecimalSeparator = culture.NumberFormat.NumberDecimalSeparator;
        GroupSeparator = culture.NumberFormat.NumberGroupSeparator;
    }

    /// <summary>
    /// Date year month day ordering like d/M/y etc.
    /// </summary>
    public string DateOrder { get; set; }

    /// <summary>
    /// Date format
    /// </summary>
    public string DateFormat { get; set; }

    /// <summary>
    /// Date separator
    /// </summary>
    public string DateSeparator { get; set; }

    /// <summary>
    /// Date/time format
    /// </summary>
    public string DateTimeFormat { get; set; }

    /// <summary>
    /// Decimal separator
    /// </summary>
    public string DecimalSeparator { get; set; }

    /// <summary>
    /// Group separator
    /// </summary>
    public string GroupSeparator { get; set; }
}