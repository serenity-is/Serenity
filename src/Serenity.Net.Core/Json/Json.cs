using System.Text.Json;

namespace Serenity;

/// <summary>
/// Contains shortcuts to Json serialization / deserialization methods, and default
/// Serenity settings.
/// </summary>
public static partial class JSON
{
    /// <summary>
    /// Deserializes a JSON string to an object
    /// </summary>
    /// <typeparam name="T">Type to deserialize</typeparam>
    /// <param name="input">JSON string</param>
    /// <returns>Deserialized object</returns>
    public static T? Deserialize<T>(string input)
    {
        return JsonSerializer.Deserialize<T>(input, JsonDefaults.StrictWriteNulls);
    }

    /// <summary>
    /// Deserializes a JSON string to an object
    /// </summary>
    /// <param name="targetType">Type to deserialize</param>
    /// <param name="input">JSON string</param>
    /// <returns>Deserialized object</returns>
    public static object? Deserialize(string input, Type targetType)
    {
        return JsonSerializer.Deserialize(input, targetType, JsonDefaults.StrictWriteNulls);
    }

    /// <summary>
    /// Deserializes a JSON string to an object, using more tolerant settings.
    /// </summary>
    /// <typeparam name="T">Type to deserialize</typeparam>
    /// <param name="input">JSON string</param>
    /// <returns>Deserialized object</returns>
    public static T? DeserializeTolerant<T>(string input)
    {
        return JsonSerializer.Deserialize<T>(input, JsonDefaults.TolerantWriteNulls);
    }

    /// <summary>
    /// Deserializes a JSON string to an object, using more tolerant settings
    /// </summary>
    /// <param name="targetType">Type to deserialize</param>
    /// <param name="input">JSON string</param>
    /// <returns>Deserialized object</returns>
    public static object? DeserializeTolerant(string input, Type targetType)
    {
        return JsonSerializer.Deserialize(input, targetType, JsonDefaults.TolerantWriteNulls);
    }   

    /// <summary>
    /// Converts object to its JSON representation
    /// </summary>
    /// <param name="value">Value to convert to JSON</param>
    /// <param name="writeNulls">If true, serializes null values.</param>
    /// <returns>Serialized JSON string</returns>
    public static string Serialize(object? value, bool writeNulls = false)
    {
        return JsonSerializer.Serialize(value, writeNulls ? JsonDefaults.StrictWriteNulls : JsonDefaults.Strict);
    }

    private static readonly JsonSerializerOptions Indented = new(JsonDefaults.Strict)
    {
        WriteIndented = true
    };

    private static readonly JsonSerializerOptions IndentedWriteNulls = new(JsonDefaults.StrictWriteNulls)
    {
        WriteIndented = true
    };

    /// <summary>
    /// Converts object to its JSON representation
    /// </summary>
    /// <param name="value">Value to convert to JSON</param>
    /// <param name="writeNulls">If true, serializes null values.</param>
    /// <returns>Serialized JSON string</returns>
    public static string SerializeIndented(object? value, bool writeNulls = false)
    {
        return JsonSerializer.Serialize(value, writeNulls ? IndentedWriteNulls : Indented);
    }
}