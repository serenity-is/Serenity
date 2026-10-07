using System.Collections;
using System.Text.Json;

namespace Serenity.Data;

/// <summary>
/// Field with a Variant (e.g. SQL VARIANT) value.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="VariantField"/> class.
/// </remarks>
/// <param name="collection">The collection.</param>
/// <param name="name">The name.</param>
/// <param name="caption">The caption.</param>
/// <param name="size">The size.</param>
/// <param name="flags">The flags.</param>
/// <param name="getValue">The get value.</param>
/// <param name="setValue">The set value.</param>
public class VariantField(ICollection<Field> collection, string name, LocalText? caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
    Func<IRow, object?>? getValue = null, Action<IRow, object?>? setValue = null) : GenericClassField<object>(collection, FieldType.String, name, caption, size, flags, getValue, setValue)
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
    /// <returns>A new VariantField instance.</returns>
    public static VariantField Factory(ICollection<Field> collection, string name, LocalText? caption, int size, FieldFlags flags,
        Func<IRow, object?>? getValue, Action<IRow, object?>? setValue)
    {
        return new VariantField(collection, name, caption, size, flags, getValue, setValue);
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
            _setValue(row, reader.GetValue(index));

        row.OnFieldSet(this);
    }

    /// <summary>
    /// Compares two values of this field using the specified comparer.
    /// </summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <param name="comparer">The comparer, or null to use the default comparer for the value type.</param>
    /// <returns>A value indicating the relative order of the two values.</returns>
    protected override int CompareValues(object value1, object value2, IComparer? comparer)
    {
        comparer ??= Comparer;

        if (value1 is string string1 && value2 is string string2 && comparer is IComparer<string> stringComparer)
            return stringComparer.Compare(string1, string2);

        if (value1.GetType() == value2.GetType() && value1 is IComparable comparable)
            return comparable.CompareTo(value2);

        return string.Compare(
            Convert.ToString(value1, CultureInfo.InvariantCulture),
            Convert.ToString(value2, CultureInfo.InvariantCulture),
            StringComparison.Ordinal);
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
            case Newtonsoft.Json.JsonToken.Integer:
            case Newtonsoft.Json.JsonToken.Float:
            case Newtonsoft.Json.JsonToken.Bytes:
            case Newtonsoft.Json.JsonToken.Boolean:
                _setValue(row, reader.Value);
                break;
            default:
                throw JsonUnexpectedToken(reader);
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
            case JsonTokenType.True:
            case JsonTokenType.False:
                _setValue(row, reader.TokenType == JsonTokenType.True);
                break;
            case JsonTokenType.Number:
                if (reader.TryGetInt32(out int intValue))
                    _setValue(row, intValue);
                else if (reader.TryGetInt64(out long longValue))
                    _setValue(row, longValue);
                else
                    _setValue(row, reader.GetDouble());
                
                break;
            case JsonTokenType.String:
                _setValue(row, reader.GetString());
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
            JsonSerializer.Serialize(writer, value, options);
    }
}
