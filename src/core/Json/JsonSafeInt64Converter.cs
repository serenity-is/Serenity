using JsonConverter = Newtonsoft.Json.JsonConverter;
using Newtonsoft.Json;

namespace Serenity.Data;

/// <summary>
/// Serializes and deserializes a row.
/// </summary>
public class JsonSafeInt64Converter : JsonConverter
{
    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
            writer.WriteNull();
        else
        {
            var intvalue = Convert.ToInt64(value);
            if (intvalue > 9007199254740992 ||
                intvalue < -9007199254740992)
                writer.WriteValue(intvalue.ToString(CultureInfo.InvariantCulture));
            else
                writer.WriteValue(intvalue);
        }
    }

    /// <summary>
    /// Reads the JSON representation of the object.
    /// </summary>
    /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
    /// <param name="objectType">The type of the object.</param>
    /// <param name="existingValue">The existing value of the object being read.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>The object value.</returns>
    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Determines whether this instance can convert the specified object type.
    /// </summary>
    /// <param name="objectType">The type of the object.</param>
    /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(long) || objectType == typeof(long?);
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="JsonConverter"/> can read JSON.
    /// </summary>
    /// <value><c>false</c>, as this converter only supports writing.</value>
    public override bool CanRead => false;

    /// <summary>
    /// Gets a value indicating whether this <see cref="JsonConverter"/> can write JSON.
    /// </summary>
    /// <value><c>true</c>, as this converter supports writing.</value>
    public override bool CanWrite => true;
}