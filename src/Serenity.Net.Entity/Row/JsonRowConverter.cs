namespace Serenity.Data;

/// <summary>
///   Serialize/deserialize a row</summary>
public class JsonRowConverter : JsonConverter
{
    /// <summary>
    /// Should serialize extension
    /// </summary>
    public static Func<IRow, string, bool> ShouldSerializeExtension;

    /// <summary>
    /// Should deserialize extension
    /// </summary>
    public static Func<IRow, string, bool> ShouldDeserializeExtension;

    /// <summary>
    ///   Writes the JSON representation of the object.</summary>
    /// <param name="writer">
    ///   The <see cref="JsonWriter"/> to write to.</param>
    /// <param name="value">
    ///   The value.</param>
    /// <param name="serializer">
    ///   The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        ToJson(writer, (IRow)value, serializer);
    }

    private void ToJson(JsonWriter writer, IRow row, JsonSerializer serializer)
    {
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
                        if (!f.IsNull(row) || serializer.NullValueHandling == NullValueHandling.Include)
                        {
                            writer.WritePropertyName(f.PropertyName ?? f.Name);
                            f.ValueToJson(writer, row, serializer);
                        }
                    }
            }
        }
        else
        {
            var fields = row.Fields;
            foreach (var f in fields)
                if (!f.IsNull(row) || serializer.NullValueHandling == NullValueHandling.Include)
                {
                    writer.WritePropertyName(f.PropertyName ?? f.Name);
                    f.ValueToJson(writer, row, serializer);
                }
        }

        if (ShouldSerializeExtension != null)
        {
            foreach (string key in row.GetDictionaryDataKeys())
            {
                if (ShouldSerializeExtension(row, key))
                {
                    writer.WritePropertyName(key);
                    writer.WriteValue(row.GetDictionaryData(key));
                }
            }
        }

        writer.WriteEndObject();
    }

    /// <summary>
    ///   Reads the JSON representation of the object.</summary>
    /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
    /// <param name="objectType">
    ///   Type of the object.</param>
    /// <param name="existingValue">
    ///   The existing value of object being read.</param>
    /// <param name="serializer">
    ///   The calling serializer.</param>
    /// <returns>
    ///   The object value.</returns>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var row = (IRow)Activator.CreateInstance(objectType);
        if (row == null)
            throw new JsonSerializationException(string.Format("No row of type {0} could be created.", objectType.Name));

        row.TrackAssignments = true;

        if (!reader.Read())
            throw new JsonSerializationException("Unexpected end when deserializing object.");

        do
        {
            switch (reader.TokenType)
            {
                case JsonToken.PropertyName:
                    string fieldName = (string)reader.Value;

                    if (!reader.Read())
                        throw new JsonSerializationException("Unexpected end when deserializing object.");

                    var field = row.Fields.FindField(fieldName);
                    if (field is null)
                        field = row.Fields.FindFieldByPropertyName(fieldName);

                    bool deserializeAsExtension = field is null &&
                        ShouldDeserializeExtension is not null && ShouldDeserializeExtension(row, fieldName);

                    if (field is null && 
                        !deserializeAsExtension &&
                        serializer.MissingMemberHandling == MissingMemberHandling.Error)
                            throw new JsonSerializationException(string.Format("Could not find field '{0}' on row of type '{1}'", fieldName, objectType.Name));

                    while (reader.TokenType == JsonToken.Comment)
                        reader.Read();

                    if (field is null)
                    {
                        if (deserializeAsExtension)
                            row.SetDictionaryData(fieldName, reader.Value);
                        else
                            reader.Skip();
                    }
                    else
                        field.ValueFromJson(reader, row, serializer);

                    break;

                case JsonToken.EndObject:
                    return row;

                default:
                    throw new JsonSerializationException("Unexpected token when deserializing row: " + reader.TokenType);
            }
        }
        while (reader.Read());

        throw new JsonSerializationException("Unexpected end when deserializing object.");
    }

    /// <summary>
    ///   Determines whether this instance can convert the specified object type.</summary>
    /// <param name="objectType">
    ///   Type of the object.</param>
    /// <returns>
    ///   True if this instance can convert the specified object type; otherwise, false.</returns>
    public override bool CanConvert(Type objectType)
    {
        return typeof(IRow).IsAssignableFrom(objectType) && !objectType.IsInterface && !objectType.IsAbstract;
    }

    /// <summary>
    ///   Gets a value indicating whether this <see cref="JsonConverter"/> can read JSON.</summary>
    /// <value>
    ///   True if this <see cref="JsonConverter"/> can write JSON; otherwise, false.</value>
    public override bool CanRead => true;

    /// <summary>
    ///   Gets a value indicating whether this <see cref="JsonConverter"/> can write JSON.</summary>
    /// <value>
    ///   True if this <see cref="JsonConverter"/> can write JSON; otherwise, false.</value>
    public override bool CanWrite => true;
}