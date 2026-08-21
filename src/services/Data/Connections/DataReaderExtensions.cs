namespace Serenity.Data;

/// <summary>
/// Extension methods for <see cref="IDataReader"/> objects.
/// </summary>
public static class DataReaderExtensions
{
    /// <summary>
    /// Reads the value at the field index. Returns <c>null</c> if the value is <see cref="DBNull"/>.
    /// </summary>
    /// <param name="reader">The reader (required).</param>
    /// <param name="index">The field index.</param>
    /// <returns>The field value, or <c>null</c> if the value is <see cref="DBNull"/>.</returns>
    public static DateTime? AsDateTime(this IDataReader reader, int index)
    {
        if (reader.IsDBNull(index))
            return null;

        return reader.GetDateTime(index);
    }

    /// <summary>
    /// Reads and converts the value at the field index to <see cref="DateTime"/>. Returns <c>null</c> if the value is <see cref="DBNull"/>.
    /// </summary>
    /// <param name="reader">The reader (required).</param>
    /// <param name="index">The field index.</param>
    /// <returns>The field value, or <c>null</c> if the value is <see cref="DBNull"/>.</returns>
    public static DateTime? ToDateTime(this IDataReader reader, int index)
    {
        if (reader.IsDBNull(index))
            return null;

        return Convert.ToDateTime(reader.GetValue(index));
    }

    /// <summary>
    /// Reads the value at the field index. Returns <c>null</c> if the value is <see cref="DBNull"/>.
    /// </summary>
    /// <param name="reader">The reader (required).</param>
    /// <param name="index">The field index.</param>
    /// <returns>The field value, or <c>null</c> if the value is <see cref="DBNull"/>.</returns>
    public static decimal? AsDecimal(this IDataReader reader, int index)
    {
        if (reader.IsDBNull(index))
            return null;

        return reader.GetDecimal(index);
    }

    /// <summary>
    /// Reads and converts the value at the field index to <see cref="decimal"/>. Returns <c>null</c> if the value is <see cref="DBNull"/>.
    /// </summary>
    /// <param name="reader">The reader (required).</param>
    /// <param name="index">The field index.</param>
    /// <returns>The field value, or <c>null</c> if the value is <see cref="DBNull"/>.</returns>
    public static decimal? ToDecimal(this IDataReader reader, int index)
    {
        if (reader.IsDBNull(index))
            return null;

        return Convert.ToDecimal(reader.GetValue(index));
    }

    /// <summary>
    /// Reads the value at the field index. Returns <c>null</c> if the value is <see cref="DBNull"/>.
    /// </summary>
    /// <param name="reader">The reader (required).</param>
    /// <param name="index">The field index.</param>
    /// <returns>The field value, or <c>null</c> if the value is <see cref="DBNull"/>.</returns>
    public static double? AsDouble(this IDataReader reader, int index)
    {
        if (reader.IsDBNull(index))
            return null;

        return reader.GetDouble(index);
    }

    /// <summary>
    /// Reads and converts the value at the field index to <see cref="double"/>. Returns <c>null</c> if the value is <see cref="DBNull"/>.
    /// </summary>
    /// <param name="reader">The reader (required).</param>
    /// <param name="index">The field index.</param>
    /// <returns>The field value, or <c>null</c> if the value is <see cref="DBNull"/>.</returns>
    public static double? ToDouble(this IDataReader reader, int index)
    {
        if (reader.IsDBNull(index))
            return null;

        return Convert.ToDouble(reader.GetValue(index));
    }

    /// <summary>
    /// Reads the value at the field index. Returns <c>null</c> if the value is <see cref="DBNull"/>.
    /// </summary>
    /// <param name="reader">The reader (required).</param>
    /// <param name="index">The field index.</param>
    /// <returns>The field value, or <c>null</c> if the value is <see cref="DBNull"/>.</returns>
    public static int? AsInt32(this IDataReader reader, int index)
    {
        if (reader.IsDBNull(index))
            return null;

        return reader.GetInt32(index);
    }

    /// <summary>
    /// Reads and converts the value at the field index to <see cref="int"/>. Returns <c>null</c> if the value is <see cref="DBNull"/>.
    /// </summary>
    /// <param name="reader">The reader (required).</param>
    /// <param name="index">The field index.</param>
    /// <returns>The field value, or <c>null</c> if the value is <see cref="DBNull"/>.</returns>
    public static int? ToInt32(this IDataReader reader, int index)
    {
        if (reader.IsDBNull(index))
            return null;

        return Convert.ToInt32(reader.GetValue(index));
    }

    /// <summary>
    /// Reads the value at the field index. Returns <c>null</c> if the value is <see cref="DBNull"/>.
    /// </summary>
    /// <param name="reader">The reader (required).</param>
    /// <param name="index">The field index.</param>
    /// <returns>The field value, or <c>null</c> if the value is <see cref="DBNull"/>.</returns>
    public static long? AsInt64(this IDataReader reader, int index)
    {
        if (reader.IsDBNull(index))
            return null;

        return reader.GetInt64(index);
    }

    /// <summary>
    /// Reads and converts the value at the field index to <see cref="long"/>. Returns <c>null</c> if the value is <see cref="DBNull"/>.
    /// </summary>
    /// <param name="reader">The reader (required).</param>
    /// <param name="index">The field index.</param>
    /// <returns>The field value, or <c>null</c> if the value is <see cref="DBNull"/>.</returns>
    public static long? ToInt64(this IDataReader reader, int index)
    {
        if (reader.IsDBNull(index))
            return null;

        return Convert.ToInt64(reader.GetValue(index));
    }

    /// <summary>
    /// Reads the value at the field index. Returns <c>null</c> if the value is <see cref="DBNull"/>.
    /// </summary>
    /// <param name="reader">The reader (required).</param>
    /// <param name="index">The field index.</param>
    /// <returns>The field value, or <c>null</c> if the value is <see cref="DBNull"/>.</returns>
    public static string AsString(this IDataReader reader, int index)
    {
        if (reader.IsDBNull(index))
            return null;

        return reader.GetString(index);
    }

    /// <summary>
    /// Reads and converts the value at the field index to <see cref="string"/>. Returns <c>null</c> if the value is <see cref="DBNull"/>.
    /// </summary>
    /// <param name="reader">The reader (required).</param>
    /// <param name="index">The field index.</param>
    /// <returns>The field value, or <c>null</c> if the value is <see cref="DBNull"/>.</returns>
    public static string ToString(this IDataReader reader, int index)
    {
        if (reader.IsDBNull(index))
            return null;

        return Convert.ToString(reader.GetValue(index));
    }
}