using System.IO;
using System.Text.Json;

namespace Serenity.Data;

/// <summary>
/// Field with a Stream value
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
public class StreamField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
    Func<IRow, Stream> getValue = null, Action<IRow, Stream> setValue = null) : GenericClassField<Stream>(collection, FieldType.Stream, name, caption, size, flags, getValue, setValue)
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
    public static StreamField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
        Func<IRow, Stream> getValue, Action<IRow, Stream> setValue)
    {
        return new StreamField(collection, name, caption, size, flags, getValue, setValue);
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
        {
            byte[] a;

            if (reader.GetType().Name == "SqliteDataReader")
            {
                a = (byte[])reader.GetValue(index);
            }
            else
            {
                long available = reader.GetBytes(index, 0, null, 0, 0);
                a = new byte[available];
                if (a.Length > 0)
                    reader.GetBytes(index, 0, a, 0, a.Length);
            }

            _setValue(row, new MemoryStream(a));
        }

        row.FieldAssignedValue(this);
    }

    /// <summary>
    /// Compares the field values for two rows for an ascending index sort
    /// </summary>
    /// <param name="row1">The row1.</param>
    /// <param name="row2">The row2.</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override int IndexCompare(IRow row1, IRow row2)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Copies the stream.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="dest">The dest.</param>
    /// <exception cref="ArgumentNullException">
    /// source
    /// or
    /// dest
    /// </exception>
    public static void CopyStream(Stream source, Stream dest)
    {
        if (source == null)
            throw new ArgumentNullException("source");
        if (dest == null)
            throw new ArgumentNullException("dest");

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
    /// Serializes this fields value to JSON
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
            var ms = new MemoryStream((int)value.Length);
            CopyStream(value, ms);
            writer.WriteValue(ms.ToArray());
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
            case Newtonsoft.Json.JsonToken.String:
                _setValue(row, new MemoryStream(Convert.FromBase64String((string)reader.Value)));
                break;
            case Newtonsoft.Json.JsonToken.Bytes:
                _setValue(row, new MemoryStream((byte[])reader.Value));
                break;
            default:
                throw JsonUnexpectedToken(reader);
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
                if (string.IsNullOrEmpty(reader.GetString()))
                    _setValue(row, null);
                else
                    _setValue(row, new MemoryStream(reader.GetBytesFromBase64()));
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
        if (value == null || value.Length == 0)
            writer.WriteNullValue();
        else
        {
            var ms = new MemoryStream((int)value.Length);
            CopyStream(value, ms);
            writer.WriteBase64StringValue(ms.ToArray());
        }
    }
}
