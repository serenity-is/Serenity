using System.Collections;
using System.Text.Json;

namespace Serenity.Data;

/// <summary>
/// Field with a byte[] value.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ByteArrayField"/> class.
/// </remarks>
/// <param name="collection">The collection.</param>
/// <param name="name">The name.</param>
/// <param name="caption">The caption.</param>
/// <param name="size">The size.</param>
/// <param name="flags">The flags.</param>
/// <param name="getValue">The get value.</param>
/// <param name="setValue">The set value.</param>
public class ByteArrayField(ICollection<Field> collection, string name, LocalText? caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
    Func<IRow, byte[]?>? getValue = null, Action<IRow, byte[]?>? setValue = null) : CustomClassField<byte[]>(collection, name, caption, size, flags, getValue, setValue)
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
    /// <returns>A new ByteArrayField instance.</returns>
    public static ByteArrayField Factory(ICollection<Field> collection, string name, LocalText? caption, int size, FieldFlags flags,
        Func<IRow, byte[]> getValue, Action<IRow, byte[]?> setValue)
    {
        return new ByteArrayField(collection, name, caption, size, flags, getValue, setValue);
    }

    /// <summary>
    /// Gets field value from a reader.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="index">The index.</param>
    /// <param name="row">The row.</param>
    /// <exception cref="ArgumentNullException">reader is null.</exception>
    public override void GetFromReader(IDataReader reader, int index, IRow row)
    {
        ArgumentNullException.ThrowIfNull(reader);

        _setValue(row, reader.AsBytes(index));

        row.OnFieldSet(this);
    }

    /// <summary>
    /// Compares the values.
    /// </summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns>A value indicating the relative order of the two values.</returns>
    protected override int CompareValues(byte[] value1, byte[] value2, IComparer? comparer)
    {
        comparer ??= Comparer;
        var byteComparer = comparer as IComparer<byte> ?? Comparer<byte>.Default;
        var length = Math.Min(value1.Length, value2.Length);
        for (var i = 0; i < length; i++)
        {
            var c = byteComparer.Compare(value1[i], value2[i]);
            if (c != 0)
                return c;
        }

        return value1.Length.CompareTo(value2.Length);
    }

    /// <summary>
    /// Clones the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A clone of the value.</returns>
    protected override byte[]? Clone(byte[]? value)
    {
        return (byte[]?)value?.Clone();
    }

    /// <summary>
    /// Serializes this field's value to JSON.
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
            writer.WriteValue(value);
        }
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
            {
                var s = (string)reader.Value!;
                var maxBase64Length = Base64Helper.GetMaxLength(Size);
                if (s.Length > maxBase64Length && Base64Helper.CountCharacters(s.AsSpan()) > maxBase64Length)
                    throw new Newtonsoft.Json.JsonSerializationException(Base64Helper.GetTooLongMessage(this));

                _setValue(row, Convert.FromBase64String(s));
                break;
            }
            case Newtonsoft.Json.JsonToken.Bytes:
            {
                var bytes = (byte[])reader.Value!;
                if (Size > 0 && bytes.Length > Size)
                    throw new Newtonsoft.Json.JsonSerializationException(Base64Helper.GetTooLongMessage(this));

                _setValue(row, bytes);
                break;
            }
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
            case JsonTokenType.String:
                if (Base64Helper.GetLength(ref reader) > Base64Helper.GetMaxLength(Size))
                    throw new JsonException(Base64Helper.GetTooLongMessage(this));

                if (reader.HasValueSequence ? reader.ValueSequence.IsEmpty : reader.ValueSpan.IsEmpty)
                    _setValue(row, null);
                else
                    _setValue(row, reader.GetBytesFromBase64());
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
            writer.WriteBase64StringValue(value);
    }

    /// <inheritdoc/>
    public override object AsSqlValue(IRow row)
    {
        var value = AsObject(row);
        if (value == null)
            return System.Data.SqlTypes.SqlBinary.Null;

        return value;
    }
}
