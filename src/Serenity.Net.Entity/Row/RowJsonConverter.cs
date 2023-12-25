using System.Text.Json;

namespace Serenity.JsonConverters;

/// <summary>
///   Serialize/deserialize a row</summary>
public class RowJsonConverter : JsonConverter<IRow>
{
    /// <summary>
    /// Should serialize extension
    /// </summary>
    public static Func<IRow, string, bool> ShouldSerializeExtension;

    /// <summary>
    /// Should deserialize extension
    /// </summary>
    public static Func<IRow, string, bool> ShouldDeserializeExtension;

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, IRow row, JsonSerializerOptions options)
    {
        if (row == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();

        if (row.TrackAssignments)
        {
            if (row.IsAnyFieldAssigned)
            {
                var fields = row.Fields;
                for (var i = 0; i < row.Fields.Count; i++)
                    if (row.IsAssigned(row.Fields[i]))
                    {
                        var f = fields[i];
                        if (!f.IsNull(row) || 
                            options.DefaultIgnoreCondition != JsonIgnoreCondition.WhenWritingNull)
                        {
                            writer.WritePropertyName(f.PropertyName ?? f.Name);
                            f.ValueToJson(writer, row, options);
                        }
                    }
            }
        }
        else
        {
            var fields = row.Fields;
            foreach (var f in fields)
                if (!f.IsNull(row) ||
                    options.DefaultIgnoreCondition != JsonIgnoreCondition.WhenWritingNull)
                {
                    writer.WritePropertyName(f.PropertyName ?? f.Name);
                    f.ValueToJson(writer, row, options);
                }
        }

        if (ShouldSerializeExtension != null)
        {
            foreach (string key in row.GetDictionaryDataKeys())
            {
                if (ShouldSerializeExtension(row, key))
                {
                    writer.WritePropertyName(key);
                    JsonSerializer.Serialize(writer, row.GetDictionaryData(key), options);
                }
            }
        }

        writer.WriteEndObject();
    }

    /// <inheritdoc/>
    public override IRow Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var row = (IRow)Activator.CreateInstance(typeToConvert) ?? 
            throw new JsonException(string.Format("No row of type {0} could be created.", 
                typeToConvert.Name));

        row.TrackAssignments = true;

        if (!reader.Read())
            throw new JsonException("Unexpected end when deserializing object.");

        do
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.PropertyName:
                    string fieldName = reader.GetString();

                    if (!reader.Read())
                        throw new JsonException("Unexpected end when deserializing object.");

                    var field = row.Fields.FindField(fieldName);
                    field ??= row.Fields.FindFieldByPropertyName(fieldName);

                    bool deserializeAsExtension = field is null &&
                        ShouldDeserializeExtension is not null && ShouldDeserializeExtension(row, fieldName);

                    if (field is null && 
                        !deserializeAsExtension &&
                        options.UnmappedMemberHandling == JsonUnmappedMemberHandling.Disallow)
                            throw new JsonException(string.Format(
                                "Could not find field '{0}' on row of type '{1}'", fieldName, 
                                    typeToConvert.Name));

                    while (reader.TokenType == JsonTokenType.Comment)
                        reader.Read();

                    if (field is null)
                    {
                        if (deserializeAsExtension)
                        {
                            object v = reader.TokenType switch
                            {
                                JsonTokenType.Null => null,
                                JsonTokenType.False => false,
                                JsonTokenType.True => true,
                                JsonTokenType.String => reader.GetString(),
                                JsonTokenType.Number => reader.GetDouble(),
                                JsonTokenType.StartArray => JsonSerializer.Deserialize<object[]>(ref reader, options),
                                JsonTokenType.StartObject => JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options),
                                _ => throw new JsonException("Unexpected error while deserializing field extension value!"),
                            };
                            row.SetDictionaryData(fieldName, v);
                        }
                        else
                            reader.Skip();
                    }
                    else
                        field.ValueFromJson(ref reader, row, options);

                    break;

                case JsonTokenType.EndObject:
                    return row;

                default:
                    throw new JsonException("Unexpected token when deserializing row: " + reader.TokenType);
            }
        }
        while (reader.Read());

        throw new JsonException("Unexpected end when deserializing object.");
    }

    /// <inheritdoc/>
    public override bool CanConvert(Type objectType)
    {
        return typeof(IRow).IsAssignableFrom(objectType) && !objectType.IsInterface && !objectType.IsAbstract;
    }
}