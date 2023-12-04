using System.Text.Json;
using System.Text.Unicode;

namespace Serenity;

/// <summary>
/// Contains shortcuts to Json serialization / deserialization methods, and default
/// Serenity settings.
/// </summary>
public static class JSON
{
    /// <summary>
    /// Deserializes a JSON string to an object
    /// </summary>
    /// <typeparam name="T">Type to deserialize</typeparam>
    /// <param name="input">JSON string</param>
    /// <param name="options">Serializer options. Defaults to StrictWriteNulls.</param>
    /// <returns>Deserialized object</returns>
    public static T? Parse<T>(string input, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(input, options ?? Defaults.StrictWriteNulls);
    }

    /// <summary>
    /// Deserializes a JSON string to an object
    /// </summary>
    /// <param name="targetType">Type to deserialize</param>
    /// <param name="options">Serializer options. Defaults to StrictWriteNulls.</param>
    /// <param name="input">JSON string</param>
    /// <returns>Deserialized object</returns>
    public static object? Parse(string input, Type targetType, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize(input, targetType, options ?? Defaults.StrictWriteNulls);
    }

    /// <summary>
    /// Deserializes a JSON string to an object, using more tolerant settings.
    /// </summary>
    /// <typeparam name="T">Type to deserialize</typeparam>
    /// <param name="input">JSON string</param>
    /// <returns>Deserialized object</returns>
    public static T? ParseTolerant<T>(string input)
    {
        return JsonSerializer.Deserialize<T>(input, Defaults.TolerantWriteNulls);
    }

    /// <summary>
    /// Deserializes a JSON string to an object, using more tolerant settings
    /// </summary>
    /// <param name="targetType">Type to deserialize</param>
    /// <param name="input">JSON string</param>
    /// <returns>Deserialized object</returns>
    public static object? ParseTolerant(string input, Type targetType)
    {
        return JsonSerializer.Deserialize(input, targetType, Defaults.TolerantWriteNulls);
    }

    /// <summary>
    /// Tries to populate an existing object similar to JsonConvert's PopulateObject
    /// </summary>
    /// <typeparam name="T">Type of the object</typeparam>
    /// <param name="target">Target object</param>
    /// <param name="jsonSource">JSON string</param>
    /// <param name="options">Serializer options</param>
    public static void PopulateObject<T>(T target, string jsonSource, JsonSerializerOptions options)
        where T : class
    {
        var json = JsonDocument.Parse(jsonSource).RootElement;
        foreach (var property in json.EnumerateObject())
            OverwriteProperty(target, property, options);
    }

    static void OverwriteProperty(object target, JsonProperty updatedProperty, JsonSerializerOptions options)
    {
        if (target is null)
            return;

        var propertyInfo = target.GetType().GetProperty(updatedProperty.Name);

        if (propertyInfo == null)
            return;

        var propertyType = propertyInfo.PropertyType;
        var parsedValue = updatedProperty.Value.Deserialize(propertyType, options);

        propertyInfo.SetValue(target, parsedValue);
    }

    /// <summary>
    /// Converts object to its JSON representation
    /// </summary>
    /// <param name="value">Value to convert to JSON</param>
    /// <param name="writeNulls">If true, serializes null values.</param>
    /// <returns>Serialized JSON string</returns>
    public static string Stringify(object? value, bool writeNulls = false)
    {
        return JsonSerializer.Serialize(value, writeNulls ? Defaults.StrictWriteNulls : Defaults.Strict);
    }

    /// <summary>
    /// Converts object to its JSON representation
    /// </summary>
    /// <param name="value">Value to convert to JSON</param>
    /// <param name="options">Serializer options.</param>
    /// <returns>Serialized JSON string</returns>
    public static string Stringify(object? value, JsonSerializerOptions options)
    {
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        return JsonSerializer.Serialize(value, options);
    }

    private static readonly JsonSerializerOptions Indented = new(Defaults.Strict)
    {
        WriteIndented = true
    };

    private static readonly JsonSerializerOptions IndentedWriteNulls = new(Defaults.StrictWriteNulls)
    {
        WriteIndented = true
    };

    /// <summary>
    /// Converts object to its JSON representation
    /// </summary>
    /// <param name="value">Value to convert to JSON</param>
    /// <param name="writeNulls">If true, serializes null values.</param>
    /// <returns>Serialized JSON string</returns>
    public static string StringifyIndented(object? value, bool writeNulls = false)
    {
        return JsonSerializer.Serialize(value, writeNulls ? IndentedWriteNulls : Indented);
    }

    /// <summary>
    ///   Converts an object to its JSON representation (extension method for Stringify)</summary>
    /// <param name="value">
    ///   Object</param>
    /// <param name="writeNulls">If true, serializes null values.</param>
    /// <returns>
    ///   JSON representation string.</returns>
    public static string ToJson(this object? value, bool writeNulls = false)
    {
        return Stringify(value, writeNulls);
    }

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

        private static JsonConverter? rowJsonConverter;

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
            options.PropertyNamingPolicy = null;
            options.ReadCommentHandling = JsonCommentHandling.Skip;

            options.Converters.Add(JsonConverters.SafeInt64JsonConverter.Instance);
            options.Converters.Add(JsonConverters.ObjectJsonConverter.Instance);
            options.Converters.Add(JsonConverters.NullableJsonConverter.Instance);
            options.Converters.Add(JsonConverters.EnumJsonConverter.Instance);

            if (rowJsonConverter == null)
            {
                // Unfortunately System.Text.Json does not read the converter type
                // from the base type's JsonConverter attribute
                var rowJsonConverterType = Type.GetType("Serenity.JsonConverters.RowJsonConverter, Serenity.Net.Entity");
                if (rowJsonConverterType != null)
                    rowJsonConverter = (JsonConverter)Activator.CreateInstance(rowJsonConverterType);
            }

            if (rowJsonConverter != null)
                options.Converters.Add(rowJsonConverter);

            if (!writeNulls)
                options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            if (!tolerant)
                options.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
            return options;
        }
    }
}