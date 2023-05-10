namespace Serenity.Data;

/// <summary>
///   Serialize/deserialize a row</summary>
public class JsonSafeInt64Converter : JsonConverter
{
    /// <summary>
    ///   Writes the JSON representation of the object.</summary>
    /// <param name="writer">
    ///   The <see cref="JsonWriter"/> to write to.</param>
    /// <param name="value">
    ///   The value.</param>
    /// <param name="serializer">
    ///   The calling serializer.</param>
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
    ///   Reads the JSON representation of the object.</summary>
    /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
    /// <param name="objectType">
    ///   Type of the object.</param>
    /// <param name="existingValue">
    ///   The existing value of object being read.</param>
    /// <param name="serializer">
    ///   The calling serializer.</param>
    /// <returns>
    ///   The object value.</returns>
    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///   Determines whether this instance can convert the specified object type.</summary>
    /// <param name="objectType">
    ///   Type of the object.</param>
    /// <returns>
    ///   True if this instance can convert the specified object type; otherwise, false.</returns>
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(long) || objectType == typeof(long?);
    }

    /// <summary>
    ///   Gets a value indicating whether this <see cref="JsonConverter"/> can read JSON.</summary>
    /// <value>
    ///   True if this <see cref="JsonConverter"/> can write JSON; otherwise, false.</value>
    public override bool CanRead => false;

    /// <summary>
    ///   Gets a value indicating whether this <see cref="JsonConverter"/> can write JSON.</summary>
    /// <value>
    ///   True if this <see cref="JsonConverter"/> can write JSON; otherwise, false.</value>
    public override bool CanWrite => true;
}