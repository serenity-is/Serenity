namespace Serenity.Data;

/// <summary>
/// Field with a byte[] value
/// </summary>
public class ByteArrayField : CustomClassField<byte[]>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ByteArrayField"/> class.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <param name="name">The name.</param>
    /// <param name="caption">The caption.</param>
    /// <param name="size">The size.</param>
    /// <param name="flags">The flags.</param>
    /// <param name="getValue">The get value.</param>
    /// <param name="setValue">The set value.</param>
    public ByteArrayField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
        Func<IRow, byte[]> getValue = null, Action<IRow, byte[]> setValue = null)
        : base(collection, name, caption, size, flags, getValue, setValue)
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
    public static ByteArrayField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
        Func<IRow, byte[]> getValue, Action<IRow, byte[]> setValue)
    {
        return new ByteArrayField(collection, name, caption, size, flags, getValue, setValue);
    }

    /// <summary>
    /// Gets field value from a reader.
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
            byte[] a;

            if (reader.GetType().Name == "SqliteDataReader")
            {
                a = (byte[])reader.GetValue(index);
            }
            else
            {
                long available = reader.GetBytes(index, 0, null, 0, 0);
                a = new byte[available];
                if (a.Length > 0)
                    reader.GetBytes(index, 0, a, 0, a.Length);
            }

            _setValue(row, a);
        }

        row.FieldAssignedValue(this);
    }

    /// <summary>
    /// Compares the values.
    /// </summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns></returns>
    protected override int CompareValues(byte[] value1, byte[] value2)
    {
        var length = Math.Min(value1.Length, value2.Length);
        for (var i = 0; i < length; i++)
        {
            var c = value1[i].CompareTo(value2[i]);
            if (c != 0)
                return c;
        }

        return value1.Length.CompareTo(value2.Length);
    }

    /// <summary>
    /// Clones the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    protected override byte[] Clone(byte[] value)
    {
        return (byte[])value.Clone();
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
        if (value == null)
            writer.WriteNull();
        else
        {
            writer.WriteValue(value);
        }
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
            case JsonToken.String:
                _setValue(row, Convert.FromBase64String((string)reader.Value));
                break;
            case JsonToken.Bytes:
                _setValue(row, (byte[])reader.Value);
                break;
            default:
                throw JsonUnexpectedToken(reader);
        }

        row.FieldAssignedValue(this);
    }
}
