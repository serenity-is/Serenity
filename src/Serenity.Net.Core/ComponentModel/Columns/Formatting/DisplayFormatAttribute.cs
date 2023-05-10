namespace Serenity.ComponentModel;

/// <summary>
/// Sets the display format for a column.
/// This has no effect on editors! It is only for Display, "NOT Editing". For editing, you have to change UI culture.
/// <para>Display format strings are specific to column data and formatter type.</para>
/// <para>If column is a Date or DateTime column, its default formatter accepts custom DateTime format strings like "dd/MM/yyyy".
/// We don't suggest setting DisplayFormat for dates explicitly, use UI culture setting in Web.config unless a column has to display date/time in a different 
/// order than the default.
/// You may also use following standard format strings:<br/>
/// - "d": "dd/MM/yyyy" where DMY order changes based on current UI culture.<br/>
/// - "g": "dd/MM/yyyy HH:mm" where DMY order changes based on current UI culture.<br/>
/// - "G": "dd/MM/yyyy HH:mm:ss" where DMY order changes based on current UI culture.<br/>
/// - "s": "yyydd-MM-ddTHH:mm:ss" ISO sortable date time format.<br/>
/// - "u": "yyydd-MM-ddTHH:mm:ss.fffZ" ISO 8601 UTC.</para>
/// <para>If column is an integer, double or decimal it accepts.NET custom numeric format strings like "#,##0.00", "0.#"</para>
/// </summary>
/// <remarks>
/// public class SomeColumns
/// {
///     [DisplayFormat("d")]
///     public DateTime DateWithCultureDMYOrder { get; set; }
///     [DisplayFormat("dd/MM/yyyy")]
///     public DateTime DateWithConstantDMYOrder { get; set; }
///     [DisplayFormat("g")]
///     public DateTime DateTimeToMinWithCultureDMYOrder { get; set; }
///     [DisplayFormat("dd/MM/yyyy HH:mm")]
///     public DateTime DateTimeToMinConstantDMYOrder { get; set; }
///     [DisplayFormat("G")]
///     public DateTime DateTimeToSecWithCultureDMYOrder { get; set; }
///     [DisplayFormat("dd/MM/yyyy HH:mm:ss")]
///     public DateTime DateTimeToSecWithConstantDMYOrder { get; set; }
///     [DisplayFormat("s")]
///     public DateTime SortableDateTime { get; set; }
///     [DisplayFormat("u")]
///     public DateTime ISO8601UTC { get; set; }
///     [DisplayFormat("#,##0.00")]
///     public Decimal ShowTwoZerosAfterDecimalWithGrouping { get; set; }
///     [DisplayFormat("0.00")]
///     public Decimal ShowTwoZerosAfterDecimalNoGrouping { get; set; }
/// }
/// </remarks>
public class DisplayFormatAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayFormatAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public DisplayFormatAttribute(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the display format value.
    /// </summary>
    /// <value>
    /// The display format value.
    /// </value>
    public string Value { get; private set; }
}