namespace Serenity.Services;

/// <summary>
///   Serialize/deserialize a SortBy object as string</summary>
public class JsonSortByConverter : JsonConverter
{
    /// <summary>
    ///   Writes the JSON representation of the object.</summary>
    /// <param name="writer">
    ///   The <see cref="JsonWriter"/> to write to.</param>
    /// <param name="value">
    ///   The value.</param>
    /// <param name="serializer">
    ///   The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var sortBy = (SortBy)value;

        string s = sortBy.Field ?? string.Empty;
        if (sortBy.Descending)
            s += " DESC";

        writer.WriteValue(s);
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
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var sortBy = new SortBy();

        if (reader.TokenType != JsonToken.String)
            throw new JsonSerializationException("Unexpected end when deserializing object.");

        var field = ((string)reader.Value).TrimToEmpty();

        if (field.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase))
        {
            sortBy.Field = field[0..^5].TrimToEmpty();
            sortBy.Descending = true;
        }
        else
            sortBy.Field = field;

        return sortBy;
    }

    /// <summary>
    ///   Determines whether this instance can convert the specified object type.</summary>
    /// <param name="objectType">
    ///   Type of the object.</param>
    /// <returns>
    ///   True if this instance can convert the specified object type; otherwise, false.</returns>
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(SortBy);
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