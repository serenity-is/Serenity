using Newtonsoft.Json.Linq;

namespace Serenity.Data;

/// <summary>
/// Field with a Guid value
/// </summary>
public sealed class GuidField : GenericValueField<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GuidField"/> class.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <param name="name">The name.</param>
    /// <param name="caption">The caption.</param>
    /// <param name="size">The size.</param>
    /// <param name="flags">The flags.</param>
    /// <param name="getValue">The get value.</param>
    /// <param name="setValue">The set value.</param>
    public GuidField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
        Func<IRow, Guid?> getValue = null, Action<IRow, Guid?> setValue = null)
        : base(collection, FieldType.Guid, name, caption, size, flags, getValue, setValue)
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
    public static GuidField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
        Func<IRow, Guid?> getValue, Action<IRow, Guid?> setValue)
    {
        return new GuidField(collection, name, caption, size, flags, getValue, setValue);
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
            _setValue(row, reader.GetGuid(index));

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
        writer.WriteValue(_getValue(row));
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
                var val = reader.Value as string;
                if (val == "")
                    _setValue(row, null);
                else
                    _setValue(row, Guid.Parse((string)reader.Value));
                break;
            default:
                throw JsonUnexpectedToken(reader);
        }

        row.FieldAssignedValue(this);
    }

    /// <summary>
    /// Converts the value.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="provider">The provider.</param>
    /// <returns></returns>
    public override object ConvertValue(object source, IFormatProvider provider)
    {
        if (source is JValue jValue)
            source = jValue.Value;

        if (source == null)
            return null;

        if (source is Guid guid)
            return guid;

        if (source is string str)
        {
            if (str == "")
                return null;

            return new Guid(str);
        }

        if (source is byte[])
            return new Guid(source as byte[]);

        return Convert.ChangeType(source, typeof(Guid), provider);
    }
}