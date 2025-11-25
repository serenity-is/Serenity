using System.Text.Json;

namespace Serenity.JsonConverters;

/// <summary>
/// Provides deserialization for object type similar to Newtonsoft
/// https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to?pivots=dotnet-8-0#deserialize-inferred-types-to-object-properties
/// </summary>
public class ObjectJsonConverter : JsonConverter<object>
{
    /// <summary>
    /// ISO 8601 date time separator position.
    /// </summary>
    private const byte IsoDateTimeSeparatorPosition = 10;
    /// <summary>
    /// Minimum ISO 8601 date time length.
    /// </summary>
    private const byte MinIsoDateTimeLength = 19;
    /// <summary>
    /// Maximum ISO 8601 date time length.
    /// </summary>
    private const byte MaxIsoDateTimeLength = 40;

    /// <summary>
    /// Default instance of the ObjectJsonConverter
    /// </summary>
    public static readonly ObjectJsonConverter Instance = new();

    /// <inheritdoc/>
    public override object Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) =>
        reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Number when reader.TryGetInt64(out long l) => l,
            JsonTokenType.Number => reader.GetDouble(),
            JsonTokenType.String when TryParseDateTimeOffset(reader.GetString(), out DateTimeOffset dto) => dto,
            JsonTokenType.String => reader.GetString()!,
            JsonTokenType.StartArray => JsonSerializer.Deserialize<object[]>(ref reader, options)!,
            JsonTokenType.StartObject => JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options)!,
            _ => JsonDocument.ParseValue(ref reader).RootElement.Clone()
        };

    /// <summary>
    /// Tries to parse a DateTimeOffset from a string.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="dateTimeOffset">The parsed DateTimeOffset.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    private static bool TryParseDateTimeOffset(string? s, out DateTimeOffset dateTimeOffset)
    {
        dateTimeOffset = default;
        if (s is null)
            return false;

        if (s.Length is < MinIsoDateTimeLength or > MaxIsoDateTimeLength ||
            !char.IsDigit(s[0]) ||
            s[IsoDateTimeSeparatorPosition] != 'T')
            return false;

        return DateTimeOffset.TryParse(s, out dateTimeOffset);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, object objectToWrite, JsonSerializerOptions options) =>
        JsonSerializer.Serialize(writer, objectToWrite, objectToWrite.GetType(), options);
}