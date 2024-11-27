using System.Text.Json;

namespace Serenity.Data;

/// <summary>
/// Field with a TimeSpan value
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TimeSpanField"/> class.
/// </remarks>
/// <param name="collection">The collection.</param>
/// <param name="name">The name.</param>
/// <param name="caption">The caption.</param>
/// <param name="size">The size.</param>
/// <param name="flags">The flags.</param>
/// <param name="getValue">The get value.</param>
/// <param name="setValue">The set value.</param>
public sealed class TimeSpanField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
    Func<IRow, TimeSpan?> getValue = null, Action<IRow, TimeSpan?> setValue = null) : GenericValueField<TimeSpan>(collection, FieldType.Time, name, caption, size, flags, getValue, setValue)
{

    /// <summary>
    /// Static factory for field, for backward compatibility, avoid using.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <param name="name">The name.</param>
    /// <param name="caption">The caption.</param>
    /// <param name="size">The size.</param>
    /// <param name="flags">The flags.</param>
    /// <param name="getValue">The get value.</param>
    /// <param name="setValue">The set value.</param>
    /// <returns></returns>
    public static TimeSpanField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
        Func<IRow, TimeSpan?> getValue, Action<IRow, TimeSpan?> setValue)
    {
        return new TimeSpanField(collection, name, caption, size, flags, getValue, setValue);
    }

    /// <summary>
    /// Gets field value from a data reader.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="index">The index.</param>
    /// <param name="row">The row.</param>
    /// <exception cref="ArgumentNullException">reader</exception>
    public override void GetFromReader(IDataReader reader, int index, IRow row)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (reader.IsDBNull(index))
            _setValue(row, null);
        else
        {
            var value = reader.GetValue(index);
            TimeSpan timeSpan;
            if (value is TimeSpan ts)
                timeSpan = ts;
            else if (value is DateTime dt)
                timeSpan = dt.TimeOfDay;
            else
                timeSpan = TimeSpan.Parse(value.ToString(), CultureInfo.InvariantCulture);

            _setValue(row, timeSpan);
        }

        row.FieldAssignedValue(this);
    }

    /// <summary>
    /// Gets or sets the <see cref="Nullable{TimeSpan}"/> with the specified row.
    /// </summary>
    /// <value>
    /// The <see cref="Nullable{TimeSpan}"/>.
    /// </value>
    /// <param name="row">The row.</param>
    /// <returns></returns>
    public new TimeSpan? this[IRow row]
    {
        get
        {
            CheckUnassignedRead(row);
            return _getValue(row);
        }
        set
        {
            if (value != null)
                _setValue(row, value.Value);
            else
                _setValue(row, null);
            row.FieldAssignedValue(this);
        }
    }

    /// <summary>
    /// Serializes this fields value to JSON
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="row">The row.</param>
    /// <param name="serializer">The serializer.</param>
    public override void ValueToJson(Newtonsoft.Json.JsonWriter writer, IRow row, Newtonsoft.Json.JsonSerializer serializer)
    {
        var value = _getValue(row);
        if (value.HasValue)
            writer.WriteValue(value.Value.ToString("c", CultureInfo.InvariantCulture));
        else
            writer.WriteNull();
    }

    /// <summary>
    /// Deserializes this fields value from JSON
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="row">The row.</param>
    /// <param name="serializer">The serializer.</param>
    /// <exception cref="ArgumentNullException">reader</exception>
    public override void ValueFromJson(Newtonsoft.Json.JsonReader reader, IRow row, Newtonsoft.Json.JsonSerializer serializer)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        switch (reader.TokenType)
        {
            case Newtonsoft.Json.JsonToken.Null:
            case Newtonsoft.Json.JsonToken.Undefined:
                _setValue(row, null);
                break;
            case Newtonsoft.Json.JsonToken.Date:
                var obj = reader.Value;
                DateTime value;
                if (obj is DateTime dt)
                    value = dt;
                else if (obj is DateTimeOffset dto)
                {
                    _setValue(row, dto.TimeOfDay);
                    break;
                }
                else
                    value = Convert.ToDateTime(obj, CultureInfo.InvariantCulture);
                _setValue(row, value.TimeOfDay);
                break;
            case Newtonsoft.Json.JsonToken.String:
                var s = ((string)reader.Value).TrimToNull();
                if (s == null)
                    _setValue(row, null);
                else
                    _setValue(row, TimeSpan.ParseExact(s, "c", CultureInfo.InvariantCulture));
                break;
            default:
                throw JsonUnexpectedToken(reader);
        }

        row.FieldAssignedValue(this);
    }

    /// <inheritdoc/>
    public override void ValueFromJson(ref Utf8JsonReader reader, IRow row, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                _setValue(row, null);
                break;
            case JsonTokenType.String:
                var s = reader.GetString().TrimToNull();
                if (s == null)
                    _setValue(row, null);
                else if (reader.TryGetDateTimeOffset(out var dofs))
                    _setValue(row, dofs.TimeOfDay);
                else if (reader.TryGetDateTime(out var dt))
                    _setValue(row, dt.TimeOfDay);
                else
                    _setValue(row, TimeSpan.ParseExact(s, "c", CultureInfo.InvariantCulture));
                break;
            default:
                throw UnexpectedJsonToken(ref reader);
        }

        row.FieldAssignedValue(this);
    }

    /// <inheritdoc/>
    public override void ValueToJson(Utf8JsonWriter writer, IRow row, JsonSerializerOptions options)
    {
        var value = _getValue(row);
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.Value.ToString("c", CultureInfo.InvariantCulture));
    }

}