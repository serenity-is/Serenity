using System.Text.Json;

namespace Serenity.JsonConverters;

/// <summary>
/// Ignores null values while deserializing. Should only be used on Value types!
/// </summary>
public class NullAsDefaultJsonConverter : JsonConverterFactory
{
    /// <summary>
    /// Default instance
    /// </summary>
    public static readonly NullAsDefaultJsonConverter Instance = new ();

    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsValueType;

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options) =>
        (JsonConverter)Activator.CreateInstance(
            typeof(MyConverter<>).MakeGenericType(
                [type]),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: [],
            culture: null);

    class MyConverter<T> : JsonConverter<T> where T : struct
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return default;

            return JsonSerializer.Deserialize<T>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}