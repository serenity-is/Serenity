using System.Text.Json;

namespace Serenity.Data;

/// <summary>
/// Field with a String value.
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
public class StringField(ICollection<Field> collection, string name, LocalText? caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
    Func<IRow, string?>? getValue = null, Action<IRow, string?>? setValue = null) : GenericClassField<string>(collection, FieldType.String, name, caption, size, flags, getValue, setValue)
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
    /// <returns>A new StringField instance.</returns>
    public static StringField Factory(ICollection<Field> collection, string name, LocalText? caption, int size, FieldFlags flags,
        Func<IRow, string?> getValue, Action<IRow, string?> setValue)
    {
        return new StringField(collection, name, caption, size, flags, getValue, setValue);
    }

    /// <summary>
    /// Gets field value from a data reader.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="index">The index.</param>
    /// <param name="row">The row.</param>
    /// <exception cref="ArgumentNullException">reader is null.</exception>
    public override void GetFromReader(IDataReader reader, int index, IRow row)
    {
        ArgumentNullException.ThrowIfNull(reader);

        if (reader.IsDBNull(index))
            _setValue(row, null);
        else
            _setValue(row, reader.GetString(index));

        row.OnFieldSet(this);
    }

    /// <summary>
    /// Serializes this field's value to JSON.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="row">The row.</param>
    /// <param name="serializer">The serializer.</param>
    public override void ValueToJson(Newtonsoft.Json.JsonWriter writer, IRow row, Newtonsoft.Json.JsonSerializer serializer)
    {
        writer.WriteValue(_getValue(row));
    }

    /// <summary>
    /// Deserializes this field's value from JSON.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="row">The row.</param>
    /// <param name="serializer">The serializer.</param>
    /// <exception cref="ArgumentNullException">reader is null.</exception>
    public override void ValueFromJson(Newtonsoft.Json.JsonReader reader, IRow row, Newtonsoft.Json.JsonSerializer serializer)
    {
        ArgumentNullException.ThrowIfNull(reader);

        switch (reader.TokenType)
        {
            case Newtonsoft.Json.JsonToken.Null:
            case Newtonsoft.Json.JsonToken.Undefined:
                _setValue(row, null);
                break;
            case Newtonsoft.Json.JsonToken.String:
                _setValue(row, (string?)reader.Value);
                break;
            case Newtonsoft.Json.JsonToken.Integer:
            case Newtonsoft.Json.JsonToken.Float:
            case Newtonsoft.Json.JsonToken.Boolean:
                _setValue(row, Convert.ToString(reader.Value, CultureInfo.InvariantCulture));
                break;
            case Newtonsoft.Json.JsonToken.Date:
                var val = reader.Value is DateTimeOffset dto ? dto.DateTime : (DateTime)reader.Value!;

                var style = serializer.DateFormatString;
                if (string.IsNullOrEmpty(style))
                    style = val.TimeOfDay == TimeSpan.Zero ? "yyyy-MM-dd" : "o";
                else if (val.TimeOfDay == TimeSpan.Zero && style.Contains("'T'"))
                    // date-only value: drop the time part after the quoted 'T' literal of a
                    // custom format, without touching quotes elsewhere (stripping them would
                    // turn literals like 'at' into format specifiers)
                    style = style.Split("'T'")[0];

                _setValue(row, val.ToString(style, CultureInfo.InvariantCulture));
                break;

            default:
                throw JsonUnexpectedToken(reader);
        }

        row.OnFieldSet(this);
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
                {
                    if (reader.TryGetInt64(out var l))
                        v = l.ToString(CultureInfo.InvariantCulture);
                    else
                        v = Convert.ToString(reader.GetDouble(), CultureInfo.InvariantCulture);
                }
                else
                    v = Convert.ToString(reader.TokenType == JsonTokenType.True, CultureInfo.InvariantCulture);
                _setValue(row, v);
                break;
            case JsonTokenType.String:
                v = reader.GetString()!;
                _setValue(row, v);
                break;
            default:
                throw UnexpectedJsonToken(ref reader);
        }

        row.OnFieldSet(this);
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
