using System.Text.Json;
using System.Text.Unicode;

namespace Serenity;

/// <summary>
/// Contains Serenity JSON serialization defaults.
/// </summary>
public static partial class JSON
{
    /// <summary>
    /// Contains default options for System.Text.Json serialization
    /// </summary>
    public static class Defaults
    {
        /// <summary>
        /// The stricter settings, raises error on missing members / reference loops, skips nulls when serializing
        /// </summary>
        public static readonly JsonSerializerOptions Strict;

        /// <summary>
        /// The stricter settings, raises error on missing members / reference loops, writes nulls
        /// </summary>
        public static readonly JsonSerializerOptions StrictWriteNulls;

        /// <summary>
        /// The tolerant settings, ignores missing members, reference loops on deserialization, skips nulls when serializing
        /// </summary>
        public static readonly JsonSerializerOptions Tolerant;

        /// <summary>
        /// The tolerant settings, ignores missing members, reference loops on deserialization, writes nulls
        /// </summary>
        public static readonly JsonSerializerOptions TolerantWriteNulls;

        static Defaults()
        {
            Strict = Populate(new JsonSerializerOptions(), tolerant: false, writeNulls: false);
            StrictWriteNulls = Populate(new JsonSerializerOptions(), tolerant: false, writeNulls: true);
            Tolerant = Populate(new JsonSerializerOptions(), tolerant: true, writeNulls: false);
            TolerantWriteNulls = Populate(new JsonSerializerOptions(), tolerant: true, writeNulls: true);
        }

        /// <summary>
        /// Creates a JsonSerializerSettings object with common values and converters.
        /// </summary>
        /// <param name="options">Options to populate with defaults</param>
        /// <param name="tolerant">True to ignore deserializing unmapped members</param>
        /// <param name="writeNulls">True to write null values</param>
        public static JsonSerializerOptions Populate(JsonSerializerOptions options,
            bool tolerant = false, bool writeNulls = false)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All);
            options.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals |
                JsonNumberHandling.AllowReadingFromString;
            options.PropertyNameCaseInsensitive = true;
            options.ReadCommentHandling = JsonCommentHandling.Skip;
            
            options.Converters.Add(JsonConverters.SafeInt64JsonConverter.Instance);
            options.Converters.Add(JsonConverters.ObjectJsonConverter.Instance);
            options.Converters.Add(JsonConverters.NullableJsonConverter.Instance);
            options.Converters.Add(new JsonStringEnumConverter());
            
            if (!writeNulls)
                options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            if (!tolerant)
                options.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
            return options;
        }
    }
}
