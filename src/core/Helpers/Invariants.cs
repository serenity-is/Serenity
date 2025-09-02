namespace Serenity;

/// <summary>
/// Invariant culture related constants and formats.
/// </summary>
public static class Invariants
{
    /// <summary>
    ///   Number format information for invariant culture</summary>
    public static readonly NumberFormatInfo NumberFormat;

    /// <summary>
    ///   Date time format information for invariant culture</summary>
    public static readonly DateTimeFormatInfo DateTimeFormat;

    /// <summary>
    ///   Constructor of the static DataHelper. Initializes default connection string and connection culture.
    /// </summary>
    static Invariants()
    {
        NumberFormat = NumberFormatInfo.InvariantInfo;
        DateTimeFormat = DateTimeFormatInfo.InvariantInfo;
    }

    /// <summary>
    /// Determines whether type of the value is an integer type (<see cref="short"/>, <see cref="int"/>, <see cref="long"/>).
    /// Avoid using this function as it is obsolete.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    ///   <c>true</c> if integer type; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsIntegerType(object value)
    {
        return value != null &&
            (value is int ||
             value is long ||
             value is short);
    }

    /// <summary>
    /// Converts value to string using invariant culture.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string ToInvariant(this int value)
    {
        return value.ToString(NumberFormat);
    }

    /// <summary>
    /// Converts value to string using invariant culture.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>Converted string.</returns>
    public static string ToInvariant(this long value)
    {
        return value.ToString(NumberFormat);
    }

    /// <summary>
    /// Converts value to string using invariant culture.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>Converted string.</returns>
    public static string ToInvariant(this double value)
    {
        return value.ToString(NumberFormat);
    }

    /// <summary>
    /// Converts value to string using invariant culture.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>Converted string.</returns>
    public static string ToInvariant(this decimal value)
    {
        return value.ToString(NumberFormat);
    }

    /// <summary>
    ///   <p>Tries to converts an ID's string representation to its numerical ID value (<see cref="long"/>).</p>
    ///   <p>Unlike <see cref="long.Parse(string)"/>, <c>null</c>, empty string and all other
    ///   invalid strings results in <see cref="long"/> value (not an exception).</p></summary>
    /// <param name="str">
    ///   String representation of an ID.</param>
    /// <returns>
    ///   Numerical ID value or Null.Int64 if null, empty, or invalid string.</returns>
    /// <seealso cref="TryParseID(string)"/>
    /// <seealso cref="long.Parse(string)"/>
    public static long? TryParseID(this string? str)
    {
        if (long.TryParse(str, out long id))
            return id;
        else
            return null;
    }

    /// <summary>
    ///   <p>Tries to converts an ID's string representation to its numerical ID value (Int64).</p>
    ///   <p>Unlike <see cref="long.Parse(string)"/>, <c>null</c>, empty string and all other
    ///   invalid strings results in <see cref="long"/> value (not an exception).</p></summary>
    /// <param name="str">
    ///   String representation of an ID.</param>
    /// <returns>
    ///   Numerical ID value or Null.Int64 if null, empty, or invalid string.</returns>
    /// <seealso cref="TryParseID(string)"/>
    /// <seealso cref="long.Parse(string)"/>
    public static int? TryParseID32(this string? str)
    {
        if (int.TryParse(str, out int id))
            return id;
        else
            return null;
    }

    /// <summary>
    ///   Converts an ID value, to its string representation.</summary>
    /// <param name="id">
    ///   ID value.</param>
    /// <returns>
    ///   If <paramref name="id"/> has <see cref="long"/> value, <c><see cref="string.Empty"/></c>, 
    ///   otherwise its string representation</returns>
    public static string IDString(this long? id)
    {
        if (id == null)
            return string.Empty;
        else
            return id.ToString();
    }
}