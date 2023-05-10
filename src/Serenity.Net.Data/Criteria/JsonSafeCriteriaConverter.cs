namespace Serenity.Data;

/// <summary>
///   Serialize/deserialize a BaseCriteria object and checks for safety of criteria expressions.</summary>
public class JsonSafeCriteriaConverter : JsonCriteriaConverter
{
    /// <summary>
    /// Reads the JSON representation of the object.
    /// </summary>
    /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing value of object being read.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>
    /// The object value.
    /// </returns>
    public override object ReadJson(JsonReader reader, Type objectType,
        object existingValue, JsonSerializer serializer)
    {
        var value = (BaseCriteria)base.ReadJson(reader, objectType, existingValue, serializer);

        if (value is null)
            return value;

        new SafeCriteriaValidator().Validate(value);

        return value;
    }
}