namespace Serenity.Services;

/// <summary>
///   Serialize/deserialize a IdentifierSet object as string</summary>
public class JsonStringHashSetConverter : JsonConverter
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
        var hashset = (HashSet<string>?)value;
        if (hashset == null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartArray();
        foreach (var s in hashset)
            writer.WriteValue(s);
        writer.WriteEndArray();
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
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var hashset = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (reader.TokenType != JsonToken.StartArray)
            throw new JsonSerializationException("Unexpected start array when deserializing object.");

        while (true)
        {
            reader.Read();
            if (reader.TokenType == JsonToken.String)
                hashset.Add((string)reader.Value!);
            else if (reader.TokenType == JsonToken.EndArray)
                break;
            else
                throw new JsonSerializationException("Unexpected token when deserializing object.");
        }

        return hashset;
    }

    /// <summary>
    ///   Determines whether this instance can convert the specified object type.</summary>
    /// <param name="objectType">
    ///   Type of the object.</param>
    /// <returns>
    ///   True if this instance can convert the specified object type; otherwise, false.</returns>
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(HashSet<string>);
    }

    /// <summary>
    ///   Gets a value indicating whether this <see cref="JsonConverter"/> can read JSON.</summary>
    /// <value>
    ///   True if this <see cref="JsonConverter"/> can write JSON; otherwise, false.</value>
    public override bool CanRead => true;

    /// <summary>
    ///   Gets a value indicating whether this <see cref="JsonConverter"/> can write JSON.</summary>
    /// <value>
    ///   True if this <see cref="JsonConverter"/> can write JSON; otherwise, false.</value>
    public override bool CanWrite => true;
}