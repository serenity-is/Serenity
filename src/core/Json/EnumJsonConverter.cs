using System.Text.Json;

namespace Serenity.JsonConverters;

/// <summary>
/// Serializes enum values as numbers while trying to handle string values when deserializing.
/// </summary>
public class EnumJsonConverter : JsonConverterFactory
{
    /// <summary>
    /// The default instance of the <see cref="EnumJsonConverter"/>.
    /// </summary>
    public static readonly EnumJsonConverter Instance = new();

    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum;

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options) =>
        (JsonConverter)Activator.CreateInstance(
            typeof(EnumValueConverter<>).MakeGenericType([type]),
            BindingFlags.Instance | BindingFlags.Public, binder: null, args: [], culture: null);

    class EnumValueConverter<T> : JsonConverter<T> where T : struct
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            static T ensureDefined(long value)
            {
                var val = (T)Enum.Parse(typeof(T), value.ToString());
                if (!IsValid(val))
                    throw new InvalidCastException(string.Format("{0} is not a valid {1} enum value!", value, typeof(T).Name));
                return val;
            }

            long l;
            switch (reader.TokenType)
            {
                case JsonTokenType.True:
                case JsonTokenType.False:
                case JsonTokenType.Number:
                    if (reader.TokenType == JsonTokenType.Number)
                        l = reader.GetInt64();
                    else
                        l = reader.TokenType == JsonTokenType.True ? 1 : 0;
                    return ensureDefined(l);

                case JsonTokenType.String:
                    string s = reader.GetString()!.Trim();
                    if (long.TryParse(s, out l))
                        return ensureDefined(l);

                    return (T)Enum.Parse(typeof(T), s, true);

                default:
                    throw new JsonException($"Unexpected token when deserializing enum type {typeof(T).FullName}: " + reader.TokenType);
            }
        }

        private static readonly bool isFlags = typeof(T).IsDefined(typeof(FlagsAttribute), inherit: false);

        // all the bits of the defined values, which any combination of the values of a flags enum is made of
        private static readonly ulong definedBits = isFlags ? Enum.GetValues(typeof(T)).Cast<T>().Aggregate(0UL, (bits, x) => bits | ToBits(x)) : 0;

        private static ulong ToBits(T value) => Enum.GetUnderlyingType(typeof(T)) == typeof(ulong) ?
            Convert.ToUInt64(value, CultureInfo.InvariantCulture) : unchecked((ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture));

        /// <summary>
        /// A value is valid when it is defined, or when it is a combination of the defined values for an enum with the
        /// <see cref="FlagsAttribute"/>, e.g. a value that is the sum of two of them, which is not defined itself.
        /// </summary>
        private static bool IsValid(T value) =>
            Enum.IsDefined(typeof(T), value) || (isFlags && (ToBits(value) & ~definedBits) == 0);

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(Convert.ToInt64(value));
        }
    }
}