using System.Text.Json;

namespace Serenity.Data;

/// <summary>
/// Field with Int64 value
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Int64Field"/> class.
/// </remarks>
/// <param name="collection">The collection.</param>
/// <param name="name">The name.</param>
/// <param name="caption">The caption.</param>
/// <param name="size">The size.</param>
/// <param name="flags">The flags.</param>
/// <param name="getValue">The get value.</param>
/// <param name="setValue">The set value.</param>
public sealed class Int64Field(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
    Func<IRow, long?> getValue = null, Action<IRow, long?> setValue = null) : GenericValueField<long>(collection, FieldType.Int64, name, caption, size, flags, getValue, setValue)
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
    public static Int64Field Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
        Func<IRow, long?> getValue, Action<IRow, long?> setValue)
    {
        return new Int64Field(collection, name, caption, size, flags, getValue, setValue);
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
            _setValue(row, Convert.ToInt64(reader.GetValue(index), CultureInfo.InvariantCulture));

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
        if (value == null)
            writer.WriteNull();
        else
        {
            var intvalue = value.Value;
            if (intvalue > 9007199254740992 ||
                intvalue < -9007199254740992)
                writer.WriteValue(intvalue.ToString(CultureInfo.InvariantCulture));
            else
                writer.WriteValue(intvalue);
        }
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
            case Newtonsoft.Json.JsonToken.Integer:
            case Newtonsoft.Json.JsonToken.Float:
            case Newtonsoft.Json.JsonToken.Boolean:
                var v = Convert.ToInt64(reader.Value, CultureInfo.InvariantCulture);
                if (EnumType == null)
                    _setValue(row, v);
                else
                    _setValue(row, Int32Field.ConvertEnumFromInt(EnumType, v));
                break;
            case Newtonsoft.Json.JsonToken.String:
                string s = ((string)reader.Value).TrimToNull();
                if (s == null)
                    _setValue(row, null);
                else if (EnumType == null)
                    _setValue(row, Convert.ToInt64(s, CultureInfo.InvariantCulture));
                else
                    _setValue(row, Int32Field.ConvertEnumFromString(EnumType, s));
                break;
            default:
                throw JsonUnexpectedToken(reader);
        }

        row.FieldAssignedValue(this);
    }

    /// <inheritdoc/>
    public override void ValueFromJson(ref Utf8JsonReader reader, IRow row, JsonSerializerOptions options)
    {
        long v;

        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                _setValue(row, null);
                break;
            case JsonTokenType.True:
            case JsonTokenType.False:
            case JsonTokenType.Number:
                if (reader.TokenType == JsonTokenType.Number)
                    v = reader.GetInt64();
                else
                    v = reader.TokenType == JsonTokenType.True ? 1L : 0L;
                if (EnumType == null)
                    _setValue(row, v);
                else
                    _setValue(row, Int32Field.ConvertEnumFromInt(EnumType, v));
                break;
            case JsonTokenType.String:
                string s = reader.GetString().TrimToNull();
                if (s == null)
                    _setValue(row, null);
                else if (EnumType == null)
                    _setValue(row, Convert.ToInt64(s, CultureInfo.InvariantCulture));
                else
                    _setValue(row, Int32Field.ConvertEnumFromString(EnumType, s));
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
            JsonSerializer.Serialize(writer, value.Value, options);
    }
}
