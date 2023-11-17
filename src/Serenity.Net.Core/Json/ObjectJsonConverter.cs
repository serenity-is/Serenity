namespace Serenity.JsonConverters;

using System.Text.Json;

/// <summary>
/// Provides deserialization for object type similar to Newtonsoft
/// https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to?pivots=dotnet-8-0#deserialize-inferred-types-to-object-properties
/// </summary>
public class ObjectJsonConverter : JsonConverter<object>
{
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
            JsonTokenType.String when reader.TryGetDateTimeOffset(out DateTimeOffset dto) => dto,
            JsonTokenType.String when reader.TryGetDateTime(out DateTime datetime) => datetime,
            JsonTokenType.String => reader.GetString()!,
            JsonTokenType.StartArray => JsonSerializer.Deserialize<object[]>(ref reader, options)!,
            JsonTokenType.StartObject => JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options)!,
            _ => JsonDocument.ParseValue(ref reader).RootElement.Clone()
        };

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, object objectToWrite, JsonSerializerOptions options) =>
        JsonSerializer.Serialize(writer, objectToWrite, objectToWrite.GetType(), options);
}