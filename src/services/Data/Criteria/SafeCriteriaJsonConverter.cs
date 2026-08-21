using System.Text.Json;

namespace Serenity.JsonConverters;

/// <summary>
/// Serializes and deserializes a <see cref="BaseCriteria"/> object and checks for the safety of criteria expressions.
/// </summary>
public class SafeCriteriaJsonConverter : CriteriaJsonConverter
{
    /// <inheritdoc/>
    public override BaseCriteria Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = base.Read(ref reader, typeToConvert, options);

        if (value is null)
            return value;

        new SafeCriteriaValidator().Validate(value);

        return value;
    }
}