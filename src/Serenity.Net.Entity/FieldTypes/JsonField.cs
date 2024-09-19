using System.Text.Json;

namespace Serenity.Data;

/// <summary>
/// Field with JSON value
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="JsonField{TValue}"/> class.
/// </remarks>
/// <param name="collection">The collection.</param>
/// <param name="name">The name.</param>
/// <param name="caption">The caption.</param>
/// <param name="size">The size.</param>
/// <param name="flags">The flags.</param>
/// <param name="getValue">The get value.</param>
/// <param name="setValue">The set value.</param>
public class JsonField<TValue>(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
    Func<IRow, TValue> getValue = null, Action<IRow, TValue> setValue = null) : GenericClassField<TValue>(collection, FieldType.Object, name, caption, size, flags, getValue, setValue)
    where TValue : class
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
    public static JsonField<TValue> Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
        Func<IRow, TValue> getValue, Action<IRow, TValue> setValue)
    {
        return new JsonField<TValue>(collection, name, caption, size, flags, getValue, setValue);
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
            _setValue(row, JsonSerializer.Deserialize<TValue>(reader.GetString(index), 
                SerializerOptions ?? JSON.Defaults.Strict));

        row.FieldAssignedValue(this);
    }

    /// <summary>
    /// Gets or sets the settings.
    /// </summary>
    /// <value>
    /// The settings.
    /// </value>
    public JsonSerializerOptions SerializerOptions { get; set; }

    /// <summary>
    /// Gets the value of this row as an SQL value.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <returns></returns>
    public override object AsSqlValue(IRow row)
    {
        var value = AsObject(row);
        if (value == null)
            return null;

        return JsonSerializer.Serialize(value, SerializerOptions ?? JSON.Defaults.Strict);
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
            return JSON.Stringify(value1, writeNulls: false)
                .CompareTo(JSON.Stringify(value2, writeNulls: false));
    }

    /// <summary>
    /// Serializes this fields value to JSON
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="row">The row.</param>
    /// <param name="serializer">The serializer.</param>
    public override void ValueToJson(Newtonsoft.Json.JsonWriter writer, IRow row, Newtonsoft.Json.JsonSerializer serializer)
    {
        serializer.Serialize(writer, _getValue(row));
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
                _setValue(row, Newtonsoft.Json.JsonConvert.DeserializeObject<TValue>(
                    (string)reader.Value, JsonSettings.StrictIncludeNulls));
                break;
            default:
                _setValue(row, serializer.Deserialize<TValue>(reader));
                break;
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
                if (typeof(TValue) == typeof(string))
                    _setValue(row, JsonSerializer.Deserialize<TValue>(ref reader, options));
                else
                    _setValue(row, JsonSerializer.Deserialize<TValue>(reader.GetString(), options));
                break;
            default:
                _setValue(row, JsonSerializer.Deserialize<TValue>(ref reader, options));
                break;
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
            JsonSerializer.Serialize(writer, value, options);
    }
}
