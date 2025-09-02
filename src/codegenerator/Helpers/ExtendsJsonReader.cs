using System.Buffers;
using System.Text.Json;

namespace Serenity.CodeGenerator;

public class ExtendsJsonReader
{
    public static TConfig Read<TConfig>(IFileSystem fileSystem, 
        string path, string extendsProp, JsonSerializerOptions options,
        Func<string, string> getDefault = null)
        where TConfig: class, new()
    {
        string readExtends(JsonDocument doc)
        {
            if (!doc.RootElement.TryGetProperty(extendsProp, out JsonElement value) ||
                value.ValueKind != JsonValueKind.String)
                return null;
            else
                return value.ToString();
        }

        var currentDoc = JsonDocument.Parse(fileSystem.ReadAllText(path));
        try
        {
            var extends = readExtends(currentDoc);
            var loop = 0;
            while (!string.IsNullOrEmpty(extends))
            {
                if (++loop > 100)
                    throw new InvalidOperationException($"Infinite Extends loop detected for json file: {path}!");

                var extendsJson = getDefault?.Invoke(extends) ??
                    fileSystem.ReadAllText(path = fileSystem.Combine(fileSystem.GetDirectoryName(path), extends));
                using var extendsDoc = JsonDocument.Parse(extendsJson);
                extends = readExtends(extendsDoc);
                var mergedDoc = MergeObjects(extendsDoc.RootElement, currentDoc.RootElement);
                currentDoc.Dispose();
                currentDoc = mergedDoc;
            }

            return currentDoc.Deserialize<TConfig>(options);
        }
        finally
        {
            currentDoc.Dispose();
        }
    }

    private static JsonDocument MergeObjects(JsonElement root1, JsonElement root2)
    {
        var outputBuffer = new ArrayBufferWriter<byte>();
        using (var jsonWriter = new Utf8JsonWriter(outputBuffer))
            MergeObjectsTo(jsonWriter, root1, root2);
        return JsonDocument.Parse(Encoding.UTF8.GetString(outputBuffer.WrittenSpan));
    }

    private static void MergeObjectsTo(Utf8JsonWriter jsonWriter,
        JsonElement root1, JsonElement root2)
    {
        if (root1.ValueKind != JsonValueKind.Object)
            throw new ArgumentOutOfRangeException(nameof(root1));

        if (root2.ValueKind != JsonValueKind.Object)
            throw new ArgumentOutOfRangeException(nameof(root2));

        jsonWriter.WriteStartObject();

        foreach (JsonProperty property in root1.EnumerateObject())
        {
            string propertyName = property.Name;
            JsonValueKind newValueKind;
            if (root2.TryGetProperty(propertyName, out JsonElement newValue))
            {
                newValueKind = newValue.ValueKind;
                jsonWriter.WritePropertyName(propertyName);
                JsonElement originalValue = property.Value;
                JsonValueKind originalValueKind = originalValue.ValueKind;
                if (newValueKind == JsonValueKind.Object &&
                    originalValueKind == JsonValueKind.Object)
                {
                    MergeObjectsTo(jsonWriter, originalValue, newValue);
                }
                else
                {
                    newValue.WriteTo(jsonWriter);
                }
            }
            else
            {
                property.WriteTo(jsonWriter);
            }
        }

        foreach (JsonProperty property in root2.EnumerateObject())
        {
            if (!root1.TryGetProperty(property.Name, out _))
            {
                property.WriteTo(jsonWriter);
            }
        }

        jsonWriter.WriteEndObject();
    }
}