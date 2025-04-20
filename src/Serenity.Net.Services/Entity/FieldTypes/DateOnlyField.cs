using System.Text.Json;

namespace Serenity.Data;

/// <summary>
/// Field with a DateOnly value
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DateOnlyField"/> class.
/// </remarks>
/// <param name="collection">The collection.</param>
/// <param name="name">The name.</param>
/// <param name="caption">The caption.</param>
/// <param name="size">The size.</param>
/// <param name="flags">The flags.</param>
/// <param name="getValue">The get value.</param>
/// <param name="setValue">The set value.</param>
public sealed class DateOnlyField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
    Func<IRow, DateOnly?> getValue = null, Action<IRow, DateOnly?> setValue = null) : GenericValueField<DateOnly>(collection, FieldType.DateOnly, name, caption, size, flags, getValue, setValue)
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
    public static DateOnlyField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
        Func<IRow, DateOnly?> getValue, Action<IRow, DateOnly?> setValue)
    {
        return new DateOnlyField(collection, name, caption, size, flags, getValue, setValue);
    }

    /// <summary>
    /// Converts the value.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="provider">The provider.</param>
    /// <returns></returns>
    public override object ConvertValue(object source, IFormatProvider provider)
    {
        if (source is Newtonsoft.Json.Linq.JValue jValue)
            source = jValue.Value;

        if (source == null)
            return null;
        else
        {
            if (source is DateOnly d)
                return d;

            if (source is DateTime dt)
                return DateOnly.FromDateTime(dt);

            if (source is DateTimeOffset dto)
                return DateOnly.FromDateTime(dto.Date);

            return Convert.ChangeType(source, typeof(DateOnly), provider);
        }
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
        ArgumentNullException.ThrowIfNull(reader);

        if (reader.IsDBNull(index))
            _setValue(row, null);
        else
        {
            var value = reader.GetValue(index);
            DateOnly datetime = value is DateTimeOffset dto ? DateOnly.FromDateTime(dto.Date) 
                : value is DateTime dt ? DateOnly.FromDateTime(dt)
                : value is DateOnly d ? d 
                : DateOnly.FromDateTime(Convert.ToDateTime(value));
            _setValue(row, datetime);
        }

        row.FieldAssignedValue(this);
    }

    /// <summary>
    /// Gets or sets the value of this field with the specified row.
    /// </summary>
    /// <param name="row">The row.</param>
    public new DateOnly? this[IRow row]
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
                _setValue(row, value);
            row.FieldAssignedValue(this);
        }
    }

    /// <summary>
    /// Sets the value of this field in specified row as object.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="value">The value.</param>
    public override void AsObject(IRow row, object value)
    {
        if (value == null)
            _setValue(row, null);
        else
            _setValue(row, (DateOnly)value);

        row.FieldAssignedValue(this);
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
        {
            var dt = value.Value;
            writer.WriteValue(dt.ToString("yyyy'-'MM'-'dd'", CultureInfo.InvariantCulture));
        }
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
        ArgumentNullException.ThrowIfNull(reader);

        switch (reader.TokenType)
        {
            case Newtonsoft.Json.JsonToken.Null:
            case Newtonsoft.Json.JsonToken.Undefined:
                _setValue(row, null);
                break;
            case Newtonsoft.Json.JsonToken.Date:
                var obj = reader.Value;
                DateOnly value;
                if (obj is DateOnly d)
                    value = d;
                else if (obj is DateTime dt)
                {
                    _setValue(row, DateOnly.FromDateTime(dt));
                    break;
                }
                else if (obj is DateTimeOffset dto)
                {
                    _setValue(row, DateOnly.FromDateTime(dto.Date));
                    break;
                }
                else
                    value = DateOnly.FromDateTime(Convert.ToDateTime(obj, CultureInfo.InvariantCulture));

                _setValue(row, value);
                break;
            case Newtonsoft.Json.JsonToken.String:
                var s = ((string)reader.Value).TrimToNull();
                if (s == null)
                    _setValue(row, null);
                else
                    _setValue(row, DateOnly.FromDateTime(Convert.ToDateTime(s, CultureInfo.InvariantCulture)));
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
                var s = reader.GetString();
                if (string.IsNullOrWhiteSpace(s))
                    _setValue(row, null);
                else if (reader.TryGetDateTimeOffset(out var dtofs))
                    _setValue(row, DateOnly.FromDateTime(dtofs.Date));
                else if (reader.TryGetDateTime(out var dt))
                    _setValue(row, DateOnly.FromDateTime(dt));
                else
                    _setValue(row, DateOnly.FromDateTime(Convert.ToDateTime(s.Trim(), CultureInfo.InvariantCulture)));
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
        {
            var dt = value.Value;
            writer.WriteStringValue(dt.ToString("yyyy'-'MM'-'dd'", CultureInfo.InvariantCulture));
        }
    }
}
