namespace Serenity.Data;

/// <summary>
/// Field with a DateTimeOffset value
/// </summary>
public sealed class DateTimeOffsetField : GenericValueField<DateTimeOffset>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeOffsetField"/> class.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <param name="name">The name.</param>
    /// <param name="caption">The caption.</param>
    /// <param name="size">The size.</param>
    /// <param name="flags">The flags.</param>
    /// <param name="getValue">The get value.</param>
    /// <param name="setValue">The set value.</param>
    public DateTimeOffsetField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
        Func<IRow, DateTimeOffset?> getValue = null, Action<IRow, DateTimeOffset?> setValue = null)
        : base(collection, FieldType.DateTime, name, caption, size, flags, getValue, setValue)
    {
    }

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
    public static DateTimeOffsetField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
        Func<IRow, DateTimeOffset?> getValue, Action<IRow, DateTimeOffset?> setValue)
    {
        return new DateTimeOffsetField(collection, name, caption, size, flags, getValue, setValue);
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
            DateTimeOffset dto;
            var value = reader.GetValue(index);
            if (value is DateTime dt)
                dto = dt;
            else if (value is DateTimeOffset dtofs)
                dto = dtofs;
            else
                dto = DateTimeOffset.Parse(value.ToString());

            _setValue(row, dto);
        }

        row.FieldAssignedValue(this);
    }

    /// <summary>
    /// Gets or sets the value of this field with the specified row.
    /// </summary>
    /// <value>
    /// The <see cref="Nullable{DateTimeOffset}"/>.
    /// </value>
    /// <param name="row">The row.</param>
    /// <returns></returns>
    public new DateTimeOffset? this[IRow row]
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
    /// Gets the value of this field in specified row as object.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <returns></returns>
    public override object AsObject(IRow row)
    {
        CheckUnassignedRead(row);
        return _getValue(row);
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
            _setValue(row, (DateTimeOffset)value);

        row.FieldAssignedValue(this);
    }

    /// <summary>
    /// Serializes this fields value to JSON
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="row">The row.</param>
    /// <param name="serializer">The serializer.</param>
    public override void ValueToJson(JsonWriter writer, IRow row, JsonSerializer serializer)
    {
        var value = _getValue(row);
        if (value.HasValue)
            writer.WriteValue(value.Value.ToString("o"));
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
    public override void ValueFromJson(JsonReader reader, IRow row, JsonSerializer serializer)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        switch (reader.TokenType)
        {
            case JsonToken.Null:
            case JsonToken.Undefined:
                _setValue(row, null);
                break;
            case JsonToken.Date:
                var obj = reader.Value;
                DateTimeOffset value;
                if (obj is DateTime dt)
                    value = dt;
                else if (obj is DateTimeOffset dto)
                    value = dto;
                else
                    value = DateTimeOffset.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture);
                _setValue(row, value);
                break;
            case JsonToken.String:
                var s = ((string)reader.Value).TrimToNull();
                if (s == null)
                    _setValue(row, null);
                else
                    _setValue(row, DateTimeOffset.Parse(s, CultureInfo.InvariantCulture));
                break;
            default:
                throw JsonUnexpectedToken(reader);
        }

        row.FieldAssignedValue(this);
    }
}