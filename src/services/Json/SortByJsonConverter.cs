using System.Text.Json;

namespace Serenity.JsonConverters;

/// <summary>
///   Serialize/deserialize a SortBy object as string</summary>
public class SortByJsonConverter : JsonConverter<SortBy>
{
    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, SortBy sortBy, JsonSerializerOptions options)
    {
        string s = sortBy.Field ?? string.Empty;
        if (sortBy.Descending)
            s += " DESC";

        writer.WriteStringValue(s);
    }

    /// <inheritdoc/>
    public override SortBy Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Unexpected end when deserializing object.");

        var field = reader.GetString().TrimToEmpty();

        if (field.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase))
        {
            field = field[0..^5].TrimToEmpty();
            return new SortBy(field, descending: true);
        }

        return new SortBy(field);
    }
}