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

        var dtf = culture.DateTimeFormat;
        AmDesignator = dtf.AMDesignator;
        PmDesignator = dtf.PMDesignator;
        TimeSeparator = dtf.TimeSeparator;
        FirstDayOfWeek = (int)dtf.FirstDayOfWeek;
        DayNames = dtf.DayNames;
        ShortDayNames = dtf.AbbreviatedDayNames;
        MinimizedDayNames = dtf.ShortestDayNames;
        MonthNames = dtf.MonthNames;
        ShortMonthNames = dtf.AbbreviatedMonthNames;
        // only sent for the languages where they differ, which are few, e.g. Polish, Russian or Greek
        MonthGenitiveNames = dtf.MonthGenitiveNames.SequenceEqual(dtf.MonthNames) ? null : dtf.MonthGenitiveNames;
        ShortMonthGenitiveNames = dtf.AbbreviatedMonthGenitiveNames.SequenceEqual(dtf.AbbreviatedMonthNames) ?
            null : dtf.AbbreviatedMonthGenitiveNames;
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

    /// <summary>
    /// Gets or sets the AM designator, e.g. <c>AM</c>.
    /// </summary>
    public string AmDesignator { get; set; }

    /// <summary>
    /// Gets or sets the PM designator, e.g. <c>PM</c>.
    /// </summary>
    public string PmDesignator { get; set; }

    /// <summary>
    /// Gets or sets the time separator, e.g. <c>:</c>.
    /// </summary>
    public string TimeSeparator { get; set; }

    /// <summary>
    /// Gets or sets the first day of the week, 0 for Sunday, 1 for Monday.
    /// </summary>
    public int FirstDayOfWeek { get; set; }

    /// <summary>
    /// Gets or sets the names of the days, starting with Sunday.
    /// </summary>
    public string[] DayNames { get; set; }

    /// <summary>
    /// Gets or sets the abbreviated names of the days, starting with Sunday, e.g. <c>Sun</c>.
    /// </summary>
    public string[] ShortDayNames { get; set; }

    /// <summary>
    /// Gets or sets the shortest names of the days, starting with Sunday, e.g. <c>Su</c>.
    /// </summary>
    public string[] MinimizedDayNames { get; set; }

    /// <summary>
    /// Gets or sets the names of the months, 12 of them and an empty one, as in .NET.
    /// </summary>
    public string[] MonthNames { get; set; }

    /// <summary>
    /// Gets or sets the abbreviated names of the months, 12 of them and an empty one, e.g. <c>Jan</c>.
    /// </summary>
    public string[] ShortMonthNames { get; set; }

    /// <summary>
    /// Gets or sets the names of the months in the genitive case, which a date with a day number uses, e.g.
    /// "17 listopada" in Polish, or null when they are the same as <see cref="MonthNames"/>.
    /// </summary>
    public string[]? MonthGenitiveNames { get; set; }

    /// <summary>
    /// Gets or sets the abbreviated names of the months in the genitive case, or null when they are the same
    /// as <see cref="ShortMonthNames"/>.
    /// </summary>
    public string[]? ShortMonthGenitiveNames { get; set; }
}
