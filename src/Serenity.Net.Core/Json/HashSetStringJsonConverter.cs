using System.Text.Json;

namespace Serenity.JsonConverters;

/// <summary>
///   Serialize/deserialize a HashSet object as string</summary>
public class HashSetStringJsonConverter : JsonConverter<HashSet<string>>
{
    /// <inheritdoc/>
    public override HashSet<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Unexpected start array when deserializing object.");

        var hashset = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        while (true)
        {
            reader.Read();
            if (reader.TokenType == JsonTokenType.String)
                hashset.Add(reader.GetString()!);
            else if (reader.TokenType == JsonTokenType.EndArray)
                break;
            else
                throw new JsonException("Unexpected token when deserializing object.");
        }

        return hashset;
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, HashSet<string> value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();
        foreach (var s in value)
            writer.WriteStringValue(s);
        writer.WriteEndArray();
    }
}