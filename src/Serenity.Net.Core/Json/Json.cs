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
        return JsonSerializer.Deserialize<T>(input, Defaults.StrictWriteNulls);
    }

    /// <summary>
    /// Deserializes a JSON string to an object
    /// </summary>
    /// <param name="targetType">Type to deserialize</param>
    /// <param name="input">JSON string</param>
    /// <returns>Deserialized object</returns>
    public static object? Deserialize(string input, Type targetType)
    {
        return JsonSerializer.Deserialize(input, targetType, Defaults.StrictWriteNulls);
    }

    /// <summary>
    /// Deserializes a JSON string to an object, using more tolerant settings.
    /// </summary>
    /// <typeparam name="T">Type to deserialize</typeparam>
    /// <param name="input">JSON string</param>
    /// <returns>Deserialized object</returns>
    public static T? DeserializeTolerant<T>(string input)
    {
        return JsonSerializer.Deserialize<T>(input, Defaults.TolerantWriteNulls);
    }

    /// <summary>
    /// Deserializes a JSON string to an object, using more tolerant settings
    /// </summary>
    /// <param name="targetType">Type to deserialize</param>
    /// <param name="input">JSON string</param>
    /// <returns>Deserialized object</returns>
    public static object? DeserializeTolerant(string input, Type targetType)
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

    static void OverwriteProperty<T>(T target, JsonProperty updatedProperty, JsonSerializerOptions options) where T : class
    {
        var propertyInfo = typeof(T).GetProperty(updatedProperty.Name);

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
    public static string Serialize(object? value, bool writeNulls = false)
    {
        return JsonSerializer.Serialize(value, writeNulls ? Defaults.StrictWriteNulls : Defaults.Strict);
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
    public static string SerializeIndented(object? value, bool writeNulls = false)
    {
        return JsonSerializer.Serialize(value, writeNulls ? IndentedWriteNulls : Indented);
    }
}