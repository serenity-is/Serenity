using System.Text.Json;

namespace Serenity.JsonConverters;

/// <summary>
/// Serializes and deserializes a <see cref="long"/> value, converting it to a string when it is
/// larger than the precision a double can safely handle.
/// </summary>
public class SafeInt64JsonConverter : JsonConverter<long>
{
    /// <summary>
    /// The default instance of the <see cref="SafeInt64JsonConverter"/>.
    /// </summary>
    public static readonly SafeInt64JsonConverter Instance = new();

    /// <inheritdoc/>
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            if (!long.TryParse(reader.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out long number))
                throw new JsonException();

            return number;
        }

        return reader.GetInt64();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
    {
        if (value > 9007199254740992 ||
            value < -9007199254740992)
            writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        else
            writer.WriteNumberValue(value);
    }
}