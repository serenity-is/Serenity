using System.Collections;
using System.IO;
using System.Text.Json;

namespace Serenity.Data;

/// <summary>
/// Field with a Stream value.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="StreamField"/> class.
/// </remarks>
/// <param name="collection">The collection.</param>
/// <param name="name">The name.</param>
/// <param name="caption">The caption.</param>
/// <param name="size">The size.</param>
/// <param name="flags">The flags.</param>
/// <param name="getValue">The get value.</param>
/// <param name="setValue">The set value.</param>
public class StreamField(ICollection<Field> collection, string name, LocalText? caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
    Func<IRow, Stream?>? getValue = null, Action<IRow, Stream?>? setValue = null) : GenericClassField<Stream>(collection, FieldType.Stream, name, caption, size, flags, getValue, setValue)
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
    /// <returns>A new StreamField instance.</returns>
    public static StreamField Factory(ICollection<Field> collection, string name, LocalText? caption, int size, FieldFlags flags,
        Func<IRow, Stream?> getValue, Action<IRow, Stream?> setValue)
    {
        return new StreamField(collection, name, caption, size, flags, getValue, setValue);
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

        var bytes = reader.AsBytes(index);

        _setValue(row, bytes == null ? null : new MemoryStream(bytes));

        row.OnFieldSet(this);
    }

    /// <summary>
    /// Compares two values of this field using the specified comparer.
    /// </summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <param name="comparer">The comparer, or null to use the default comparer for the value type.</param>
    /// <returns>A value indicating the relative order of the two values.</returns>
    /// <exception cref="NotImplementedException">This method is not implemented.</exception>
    protected override int CompareValues(Stream value1, Stream value2, IComparer? comparer)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Copies the stream.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="dest">The destination.</param>
    /// <exception cref="ArgumentNullException">
    /// source
    /// or
    /// dest
    /// </exception>
    public static void CopyStream(Stream source, Stream dest)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(dest);

        byte[] buffer = new byte[4096];
        int read;
        do
        {
            read = source.Read(buffer, 0, buffer.Length);
            if (read != 0)
                dest.Write(buffer, 0, read);
        } while (read != 0);
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
        if (value == null ||
            value.Length == 0)
            writer.WriteNull();
        else
        {
            var position = value.CanSeek ? value.Position : (long?)null;
            try
            {
                var ms = new MemoryStream((int)value.Length);
                CopyStream(value, ms);
                writer.WriteValue(ms.ToArray());
            }
            finally
            {
                if (position.HasValue)
                    value.Position = position.Value;
            }
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

                _setValue(row, new MemoryStream(Convert.FromBase64String(s)));
                break;
            }
            case Newtonsoft.Json.JsonToken.Bytes:
            {
                var bytes = (byte[])reader.Value!;
                if (Size > 0 && bytes.Length > Size)
                    throw new Newtonsoft.Json.JsonSerializationException(Base64Helper.GetTooLongMessage(this));

                _setValue(row, new MemoryStream(bytes));
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
                    _setValue(row, new MemoryStream(reader.GetBytesFromBase64()));
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
        if (value == null || value.Length == 0)
            writer.WriteNullValue();
        else
        {
            var position = value.CanSeek ? value.Position : (long?)null;
            try
            {
                var ms = new MemoryStream((int)value.Length);
                CopyStream(value, ms);
                writer.WriteBase64StringValue(ms.ToArray());
            }
            finally
            {
                if (position.HasValue)
                    value.Position = position.Value;
            }
        }
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
