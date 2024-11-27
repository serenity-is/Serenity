using System.Text.Json;

namespace Serenity.Data;

/// <summary>
/// Field with a String value
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="StringField"/> class.
/// </remarks>
/// <param name="collection">The collection.</param>
/// <param name="name">The name.</param>
/// <param name="caption">The caption.</param>
/// <param name="size">The size.</param>
/// <param name="flags">The flags.</param>
/// <param name="getValue">The get value.</param>
/// <param name="setValue">The set value.</param>
public class StringField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
    Func<IRow, string> getValue = null, Action<IRow, string> setValue = null) : GenericClassField<string>(collection, FieldType.String, name, caption, size, flags, getValue, setValue)
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
    public static StringField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
        Func<IRow, string> getValue, Action<IRow, string> setValue)
    {
        return new StringField(collection, name, caption, size, flags, getValue, setValue);
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
            _setValue(row, reader.GetString(index));

        row.FieldAssignedValue(this);
    }

    /// <summary>
    /// Compares the field values for two rows for an ascending index sort
    /// </summary>
    /// <param name="row1">The row1.</param>
    /// <param name="row2">The row2.</param>
    /// <returns></returns>
    public override int IndexCompare(IRow row1, IRow row2)
    {
        var value1 = _getValue(row1);
        var value2 = _getValue(row2);

        bool null1 = value1 == null;
        bool null2 = value2 == null;
        if (null1 || null2)
        {
            if (null1 && null2)
                return 0;
            else if (null1)
                return -1;
            else
                return 1;
        }
        else
            return value1.CompareTo(value2);
    }

    /// <summary>
    /// Serializes this fields value to JSON
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="row">The row.</param>
    /// <param name="serializer">The serializer.</param>
    public override void ValueToJson(Newtonsoft.Json.JsonWriter writer, IRow row, Newtonsoft.Json.JsonSerializer serializer)
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
            case Newtonsoft.Json.JsonToken.String:
                _setValue(row, (string)reader.Value);
                break;
            case Newtonsoft.Json.JsonToken.Integer:
            case Newtonsoft.Json.JsonToken.Float:
            case Newtonsoft.Json.JsonToken.Boolean:
                _setValue(row, Convert.ToString(reader.Value, CultureInfo.InvariantCulture));
                break;
            case Newtonsoft.Json.JsonToken.Date:
                var val = reader.Value is DateTimeOffset dto ? dto.DateTime : (DateTime)reader.Value!;

                var style = serializer.DateFormatString ?? "o";

                if (val.TimeOfDay == TimeSpan.Zero)
                    style = style.Contains('T') ? style.Split('T')[0] : "yyyy-MM-dd";

                if (style.Contains('\''))
                    style = style.Replace("'", "");

                _setValue(row, val.ToString(style, CultureInfo.InvariantCulture));

                break;
            default:
                throw JsonUnexpectedToken(reader);
        }

        row.FieldAssignedValue(this);
    }

    /// <inheritdoc/>
    public override void ValueFromJson(ref Utf8JsonReader reader, IRow row, JsonSerializerOptions options)
    {
        string v;

        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                _setValue(row, null);
                break;
            case JsonTokenType.True:
            case JsonTokenType.False:
            case JsonTokenType.Number:
                if (reader.TokenType == JsonTokenType.Number)
                    v = Convert.ToString(reader.GetDouble(), CultureInfo.InvariantCulture);
                else
                    v = Convert.ToString(reader.TokenType == JsonTokenType.True, CultureInfo.InvariantCulture);
                _setValue(row, v);
                break;
            case JsonTokenType.String:
                v = reader.GetString();
                _setValue(row, v);
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
            writer.WriteStringValue(value);
    }

}
