using System.Text.Json;

namespace Serenity.Data;

/// <summary>
/// Base class for custom fields with reference type values
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <seealso cref="GenericClassField{TValue}" />
/// <remarks>
/// Initializes a new instance of the <see cref="CustomClassField{TValue}"/> class.
/// </remarks>
/// <param name="collection">The collection.</param>
/// <param name="name">The name.</param>
/// <param name="caption">The caption.</param>
/// <param name="size">The size.</param>
/// <param name="flags">The flags.</param>
/// <param name="getValue">The get value.</param>
/// <param name="setValue">The set value.</param>
public class CustomClassField<TValue>(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
    Func<IRow, TValue> getValue, Action<IRow, TValue> setValue) : GenericClassField<TValue>(collection, FieldType.Object, name, caption, size, flags, getValue, setValue)
    where TValue : class
{

    /// <summary>
    /// Gets field value from a data reader.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="index">The index.</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected virtual TValue GetFromReader(IDataReader reader, int index)
    {
        throw new NotImplementedException();
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
            _setValue(row, GetFromReader(reader, index));

        row.FieldAssignedValue(this);
    }

    /// <summary>
    /// Compares the values.
    /// </summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns></returns>
    protected virtual int CompareValues(TValue value1, TValue value2)
    {
        return Comparer<TValue>.Default.Compare(value1, value2);
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
            return CompareValues(value1, value2);
    }

    /// <summary>
    /// Serializes this fields value to JSON
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The serializer.</param>
    public virtual void ValueToJson(Newtonsoft.Json.JsonWriter writer, TValue value, Newtonsoft.Json.JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
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
            ValueToJson(writer, value, serializer);
    }

    /// <summary>
    /// Deserializes this fields value from JSON
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="serializer">The serializer.</param>
    /// <returns></returns>
    protected virtual TValue ValueFromJson(Newtonsoft.Json.JsonReader reader, Newtonsoft.Json.JsonSerializer serializer)
    {
        return serializer.Deserialize<TValue>(reader);
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

            default:
                _setValue(row, ValueFromJson(reader, serializer));
                break;
        }

        row.FieldAssignedValue(this);
    }

    /// <summary>
    /// Deserializes this fields value from JSON
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="options">Serializer options</param>
    protected virtual TValue ValueFromJson(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<TValue>(ref reader, options);
    }

    /// <inheritdoc/>
    public override void ValueFromJson(ref Utf8JsonReader reader, IRow row, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                _setValue(row, null);
                break;

            default:
                _setValue(row, ValueFromJson(ref reader, options));
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
            ValueToJson(writer, value, options);
    }

    /// <summary>
    /// Serializes the value to json
    /// </summary>
    /// <param name="writer">Writer</param>
    /// <param name="value">Value</param>
    /// <param name="options">Serializer options</param>
    public virtual void ValueToJson(Utf8JsonWriter writer, TValue value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }

    /// <summary>
    /// Clones the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    protected virtual TValue Clone(TValue value)
    {
        return value;
    }

    /// <summary>
    /// Copies the specified source.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="target">The target.</param>
    public override void Copy(IRow source, IRow target)
    {
        var value = _getValue(source);
        if (value != null)
            value = Clone(value);

        _setValue(target, value);
        target.FieldAssignedValue(this);
    }
}