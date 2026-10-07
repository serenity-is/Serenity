using System.Collections;
using System.Text.Json;

namespace Serenity.Data;

/// <summary>
/// Field with a JSON value.
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
public class JsonField<TValue>(ICollection<Field> collection, string name, LocalText? caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
    Func<IRow, TValue?>? getValue = null, Action<IRow, TValue?>? setValue = null) : GenericClassField<TValue>(collection, FieldType.Object, name, caption, size, flags, getValue, setValue)
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
    /// <returns>A new JsonField instance.</returns>
    public static JsonField<TValue> Factory(ICollection<Field> collection, string name, LocalText? caption, int size, FieldFlags flags,
        Func<IRow, TValue?> getValue, Action<IRow, TValue?> setValue)
    {
        return new JsonField<TValue>(collection, name, caption, size, flags, getValue, setValue);
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
            _setValue(row, JsonSerializer.Deserialize<TValue>(reader.GetString(index), 
                SerializerOptions ?? JSON.Defaults.Strict));

        row.OnFieldSet(this);
    }

    /// <summary>
    /// Gets or sets the settings.
    /// </summary>
    /// <value>
    /// The settings.
    /// </value>
    public JsonSerializerOptions? SerializerOptions { get; set; }

    /// <summary>
    /// Gets the value of this row as an SQL value.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <returns>The value of the field in the row as an SQL value.</returns>
    public override object? AsSqlValue(IRow row)
    {
        var value = AsObject(row);
        if (value == null)
            return null;

        return JsonSerializer.Serialize(value, SerializerOptions ?? JSON.Defaults.Strict);
    }

    /// <summary>
    /// Compares two values of this field using the specified comparer.
    /// </summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <param name="comparer">The comparer, or null to use the default comparer for the value type.</param>
    /// <returns>A value indicating the relative order of the two values.</returns>
    protected override int CompareValues(TValue value1, TValue value2, IComparer? comparer)
    {
        comparer ??= Comparer;
        var jsonComparer = comparer as IComparer<string> ?? StringComparer.CurrentCulture;
        return jsonComparer.Compare(
            JSON.Stringify(value1, writeNulls: false),
            JSON.Stringify(value2, writeNulls: false));
    }

    /// <summary>
    /// Serializes this field's value to JSON.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="row">The row.</param>
    /// <param name="serializer">The serializer.</param>
    public override void ValueToJson(Newtonsoft.Json.JsonWriter writer, IRow row, Newtonsoft.Json.JsonSerializer serializer)
    {
        serializer.Serialize(writer, _getValue(row));
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
                _setValue(row, Newtonsoft.Json.JsonConvert.DeserializeObject<TValue>(
                    (string)reader.Value!, JsonSettings.StrictIncludeNulls));
                break;
            default:
                _setValue(row, serializer.Deserialize<TValue>(reader));
                break;
        }

        row.OnFieldSet(this);
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
                    _setValue(row, JsonSerializer.Deserialize<TValue>(reader.GetString()!, options));
                break;
            default:
                _setValue(row, JsonSerializer.Deserialize<TValue>(ref reader, options));
                break;
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
            JsonSerializer.Serialize(writer, value, options);
    }
}
