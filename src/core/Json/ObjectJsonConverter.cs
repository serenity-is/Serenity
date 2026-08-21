using System.Text.Json;

namespace Serenity.JsonConverters;

/// <summary>
/// Provides deserialization for the <see cref="object"/> type, inferring the concrete type
/// from the JSON value, similar to Newtonsoft.
/// See https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to?pivots=dotnet-8-0#deserialize-inferred-types-to-object-properties
/// </summary>
public class ObjectJsonConverter : JsonConverter<object>
{
    /// <summary>
    /// The position of the ISO 8601 date time separator.
    /// </summary>
    private const int IsoDateTimeSeparatorPosition = 10;
    /// <summary>
    /// The minimum length of an ISO 8601 date time string.
    /// </summary>
    private const int MinIsoDateTimeLength = 19;
    /// <summary>
    /// The maximum length of an ISO 8601 date time string.
    /// </summary>
    private const int MaxIsoDateTimeLength = 40;

    /// <summary>
    /// The default instance of the <see cref="ObjectJsonConverter"/>.
    /// </summary>
    public static readonly ObjectJsonConverter Instance = new();

    /// <inheritdoc/>
    public override object Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        var str = reader.TokenType == JsonTokenType.String ? reader.GetString() : null;
        return reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Number when reader.TryGetInt64(out long l) => l,
            JsonTokenType.Number => reader.GetDouble(),
            JsonTokenType.String when TryParseDateTimeOffset(str, out DateTimeOffset dto) => dto,
            JsonTokenType.String => str!,
            JsonTokenType.StartArray => JsonSerializer.Deserialize<object[]>(ref reader, options)!,
            JsonTokenType.StartObject => JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options)!,
            _ => JsonDocument.ParseValue(ref reader).RootElement.Clone()
        };
    }

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

        return DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeOffset);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, object objectToWrite, JsonSerializerOptions options) =>
        JsonSerializer.Serialize(writer, objectToWrite, objectToWrite.GetType(), options);
}