using System.IO;

namespace Serenity;

/// <summary>
/// Contains shortcuts to Newtonsoft.Json serialization / deserialization methods, and default
/// Serenity settings.
/// </summary>
public static class JSON
{
    /// <summary>
    /// Deserializes a JSON string to an object
    /// </summary>
    /// <typeparam name="T">Type to deserialize</typeparam>
    /// <param name="input">JSON string</param>
    /// <param name="includeNulls">If true, if a value is null and target property is not nullable, raises error.</param>
    /// <returns>Deserialized object</returns>
    public static T? Parse<T>(string input, bool includeNulls = false)
    {
        return JsonConvert.DeserializeObject<T>(input, includeNulls ? JsonSettings.StrictIncludeNulls : JsonSettings.Strict);
    }

    /// <summary>
    /// Deserializes a JSON string to an object
    /// </summary>
    /// <param name="targetType">Type to deserialize</param>
    /// <param name="input">JSON string</param>
    /// <param name="includeNulls">If true, if a value is null and target property is not nullable, raises error.</param>
    /// <returns>Deserialized object</returns>
    public static object? Parse(string input, Type targetType, bool includeNulls = false)
    {
        return JsonConvert.DeserializeObject(input, targetType, includeNulls ? JsonSettings.StrictIncludeNulls : JsonSettings.Strict);
    }

    /// <summary>
    /// Deserializes a JSON string to an object, using more tolerant settings.
    /// </summary>
    /// <typeparam name="T">Type to deserialize</typeparam>
    /// <param name="input">JSON string</param>
    /// <param name="includeNulls">If true, if a value is null and target property is not nullable, raises error.</param>
    /// <returns>Deserialized object</returns>
    public static T? ParseTolerant<T>(string input, bool includeNulls = false)
    {
        return JsonConvert.DeserializeObject<T>(input, includeNulls ? JsonSettings.TolerantIncludeNulls : JsonSettings.Tolerant);
    }

    /// <summary>
    /// Deserializes a JSON string to an object, using more tolerant settings
    /// </summary>
    /// <param name="targetType">Type to deserialize</param>
    /// <param name="input">JSON string</param>
    /// <param name="includeNulls">If true, if a value is null and target property is not nullable, raises error.</param>
    /// <returns>Deserialized object</returns>
    public static object? ParseTolerant(string input, Type targetType, bool includeNulls = false)
    {
        return JsonConvert.DeserializeObject(input, targetType, includeNulls ? JsonSettings.TolerantIncludeNulls : JsonSettings.Tolerant);
    }

    /// <summary>
    /// Converts object to its JSON representation
    /// </summary>
    /// <param name="value">Value to convert to JSON</param>
    /// <param name="includeNulls">If true, serializes null values.</param>
    /// <returns>Serialized JSON string</returns>
    public static string Stringify(object? value, bool includeNulls = false)
    {
        return JsonConvert.SerializeObject(value, includeNulls ? JsonSettings.StrictIncludeNulls : JsonSettings.Strict);
    }

    /// <summary>
    /// Converts object to its JSON representation
    /// </summary>
    /// <param name="value">Value to convert to JSON</param>
    /// <param name="indentation">Indentation (default 4)</param>
    /// <param name="includeNulls">If true, serializes null values.</param>
    /// <returns>Serialized JSON string</returns>
    public static string StringifyIndented(object? value, int indentation = 4, bool includeNulls = false)
    {
        using var sw = new StringWriter();
        using var jw = new JsonTextWriter(sw)
        {
            Formatting = Formatting.Indented,
            IndentChar = ' ',
            Indentation = indentation
        };

        var serializer = JsonSerializer.Create(includeNulls ? JsonSettings.StrictIncludeNulls : JsonSettings.Strict);
        serializer.Serialize(jw, value);
        return sw.ToString();
    }

    /// <summary>
    ///   Converts an object to its JSON representation (extension method for Stringify)</summary>
    /// <param name="value">
    ///   Object</param>
    /// <param name="includeNulls">If true, serializes null values.</param>
    /// <returns>
    ///   JSON representation string.</returns>
    /// <remarks>
    ///   null, Int32, Boolean, DateTime, Decimal, Double, Guid types handled automatically.
    ///   If object has a ToJson method it is used, otherwise value.ToString() is used as last fallback.</remarks>
    public static string ToJson(this object? value, bool includeNulls = false)
    {
        return Stringify(value, includeNulls);
    }
}