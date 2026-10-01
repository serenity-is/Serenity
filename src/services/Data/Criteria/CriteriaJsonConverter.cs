using System.Collections;
using System.Numerics;
using System.Text.Json;

namespace Serenity.JsonConverters;

/// <summary>
/// Serializes and deserializes a <see cref="BaseCriteria"/> object.
/// </summary>
public class CriteriaJsonConverter : JsonConverter<BaseCriteria>
{
    static CriteriaJsonConverter()
    {
        KeyToOperator = new Dictionary<string, CriteriaOperator>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < OperatorToKey.Length; i++)
            KeyToOperator[OperatorToKey[i]] = (CriteriaOperator)i;
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, BaseCriteria criteria, JsonSerializerOptions options)
    {
        if (criteria is null)
        {
            writer.WriteNullValue();
            return;
        }

        if (criteria.IsEmpty)
        {
            // Empty criteria is [] (JSON null stays C# null), so the two stay distinct.
            writer.WriteStartArray();
            writer.WriteEndArray();
            return;
        }

        if (criteria is ValueCriteria valueCriteria)
        {
            var value = valueCriteria.Value;
            if (value != null && value is IEnumerable && value is not string)
            {
                // make sure that first value in array won't be recognized as a operator
                // while deserializing (e.g. if values are [">", "a", "b"], serialize them
                // as [[">", "a", "b"]] so that the first > won't be recognized as GE operator.
                writer.WriteStartArray();
                JsonSerializer.Serialize(writer, value, options);
                writer.WriteEndArray();
                return;
            }

            JsonSerializer.Serialize(writer, value, options);
            return;
        }

        if (criteria is ParamCriteria prm)
        {
            writer.WriteStartArray();
            JsonSerializer.Serialize(writer, prm.Name, options);
            writer.WriteEndArray();
            return;
        }

        if (criteria is Criteria crit)
        {
            writer.WriteStartArray();
            JsonSerializer.Serialize(writer, crit.Expression, options);
            writer.WriteEndArray();
            return;
        }

        if (criteria is UnaryCriteria unary)
        {
            writer.WriteStartArray();
            writer.WriteStringValue(OperatorToKey[(int)unary.Operator]);
            Write(writer, unary.Operand, options);
            writer.WriteEndArray();
            return;
        }

        if (criteria is BinaryCriteria binary)
        {
            writer.WriteStartArray();
            Write(writer, binary.LeftOperand, options);
            writer.WriteStringValue(OperatorToKey[(int)binary.Operator]);
            Write(writer, binary.RightOperand, options);
            // The flag is only ever set when the mask needs it (see the
            // BinaryCriteria constructor), so older 3-element readers keep
            // working otherwise.
            if (binary.LikeEscapeChar is char escapeChar)
                writer.WriteStringValue(escapeChar.ToString());
            writer.WriteEndArray();
            return;
        }

        if (criteria is FunctionCallCriteria functionCall)
        {
            var functionName = functionCall.GetFunctionName(SqlSettings.DefaultDialect);
            writer.WriteStartArray();
            writer.WriteStringValue("function");
            writer.WriteStartArray();
            writer.WriteStringValue(functionName);
            foreach (var argument in functionCall.Arguments)
                Write(writer, argument, options);
            writer.WriteEndArray();
            writer.WriteEndArray();
            return;
        }

        throw new JsonException(string.Format("Can't serialize criteria of type {0}", criteria.GetType().FullName));
    }

    /// <inheritdoc/>
    public override BaseCriteria? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType is JsonTokenType.String or JsonTokenType.Number or
            JsonTokenType.True or JsonTokenType.False)
        {
            // Write emits scalar ValueCriteria as bare scalars (e.g. "test", 5),
            // so accept them back as ValueCriteria for round-trip symmetry.
            var element = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
            if (element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined)
                return null;

            return new ValueCriteria(ParseScalarValue(element));
        }

        var value = JsonSerializer.Deserialize<JsonElement[]>(ref reader, options);
        return Parse(value, allowEmpty: true);
    }

    private BaseCriteria ParseValue(JsonElement value)
    {
        if (value.ValueKind == JsonValueKind.Array)
            return Parse(JsonSerializer.Deserialize<JsonElement[]>(value), allowEmpty: false);

        if (value.ValueKind is JsonValueKind.String or JsonValueKind.Number or
            JsonValueKind.True or JsonValueKind.False or JsonValueKind.Null or JsonValueKind.Undefined)
            return new ValueCriteria(ParseScalarValue(value));

        throw new JsonException(string.Format("Can't deserialize {0} as Criteria value", value.ToString()));
    }

    private static object? ParseScalarValue(JsonElement value)
    {
        switch (value.ValueKind)
        {
            case JsonValueKind.String:
                var text = value.GetString();
                if (text is null)
                    return null;

                // Newtonsoft's default reader interprets ISO 8601 date strings
                // as DateTime values. Restrict recognition to JSON-style ISO
                // dates so ordinary strings that DateTime.TryParse accepts are
                // not silently converted. GUIDs remain strings, as JSON has no
                // scalar token distinction between a Guid and its string form.
                if (text.Length >= 10 && text[4] == '-' && text[7] == '-' &&
                    (text.Length == 10 || text[10] is 'T' or 't') &&
                    DateTime.TryParse(text, CultureInfo.InvariantCulture,
                        DateTimeStyles.RoundtripKind, out var dateTime))
                    return dateTime;

                return text;

            case JsonValueKind.Number:
                var rawNumber = value.GetRawText();
                if (rawNumber.IndexOfAny(['.', 'e', 'E']) < 0)
                {
                    if (value.TryGetInt64(out var longValue))
                        return longValue;

                    if (BigInteger.TryParse(rawNumber, NumberStyles.AllowLeadingSign,
                        CultureInfo.InvariantCulture, out var bigInteger))
                        return bigInteger;
                }

                return value.GetDouble();

            case JsonValueKind.True:
                return true;

            case JsonValueKind.False:
                return false;

            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return null;

            default:
                throw new JsonException(string.Format("Can't deserialize {0} as Criteria value", value.ToString()));
        }
    }

    private BaseCriteria Parse(JsonElement[]? array, bool allowEmpty)
    {
        if (array == null || array.Length == 0)
        {
            if (allowEmpty)
                return Criteria.Empty;
            throw new JsonException("Can't deserialize empty array as nested Criteria");
        }

        if (array.Length == 1)
        {
            if (array[0] is { ValueKind: JsonValueKind.Array } jArray)
            {
                var list = new List<object?>();
                foreach (var item in jArray.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Null ||
                        item.ValueKind == JsonValueKind.Undefined)
                    {
                        list.Add(null);
                        continue;
                    }

                    if (item.ValueKind is JsonValueKind.String or JsonValueKind.Number or
                        JsonValueKind.True or JsonValueKind.False)
                        list.Add(ParseScalarValue(item));
                    else
                        throw new JsonException(string.Format("Can't deserialize {0} as Criteria value", item.ToString()));
                }

                return new ValueCriteria(list.ToArray());
            }

            if (array[0].ValueKind != JsonValueKind.String)
                throw new JsonException(string.Format("Couldn't deserialize string criteria: {0}", array.ToString()));

            var value = array[0].GetString() ?? throw new JsonException(string.Format("Null Criteria expression: {0}", array.ToString()));
            if (value.StartsWith("@", StringComparison.Ordinal))
                return new ParamCriteria(value);

            return new Criteria(value);
        }

        if (array.Length == 2)
        {
            if (array[0].ValueKind != JsonValueKind.String)
                throw new JsonException(string.Format("Couldn't deserialize unary criteria: {0}", array.ToString()));

            var opStr = array[0].GetString();
            if (string.Equals(opStr, "function", StringComparison.OrdinalIgnoreCase))
            {
                if (array[1].ValueKind != JsonValueKind.Array)
                    throw new JsonException("Function criteria payload must be an array.");

                var functionParts = array[1].EnumerateArray().ToArray();
                if (functionParts.Length == 0 || functionParts[0].ValueKind != JsonValueKind.String ||
                    string.IsNullOrWhiteSpace(functionParts[0].GetString()))
                    throw new JsonException($"Invalid function criteria name: {array}");

                var functionName = functionParts[0].GetString()!;
                var arguments = functionParts.Skip(1).Select(ParseValue).ToArray();

                return FunctionCallCriteriaFactory.Create(functionName, arguments) ??
                    throw new JsonException($"Function '{functionName}' isn't supported in criteria JSON.");
            }

            if (!KeyToOperator.TryGetValue(opStr!, out CriteriaOperator op))
                throw new JsonException(string.Format("Unknown Criteria operator: {0}", opStr));

            if (op < CriteriaOperator.Paren || op > CriteriaOperator.Exists)
                throw new JsonException(string.Format("Invalid Unary Criteria format: {0}", array.ToString()));

            return new UnaryCriteria(op, ParseValue(array[1]));
        }

        if (array.Length == 3)
        {
            if (array[1].ValueKind != JsonValueKind.String)
                throw new JsonException(string.Format("Couldn't deserialize unary criteria: {0}", array.ToString()));

            var opStr = array[1].GetString();

            if (!KeyToOperator.TryGetValue(opStr!, out CriteriaOperator op))
                throw new JsonException(string.Format("Unknown Criteria operator: {0}", opStr));

            if (op < CriteriaOperator.AND || op > CriteriaOperator.NotLike)
                throw new JsonException(string.Format("Invalid Criteria format: {0}", array.ToString()));

            return new BinaryCriteria(ParseValue(array[0]), op, ParseValue(array[2]));
        }

        if (array.Length == 4)
        {
            if (array[1].ValueKind != JsonValueKind.String)
                throw new JsonException(string.Format("Couldn't deserialize binary criteria: {0}", array.ToString()));

            var opStr = array[1].GetString();

            if (!KeyToOperator.TryGetValue(opStr!, out CriteriaOperator op))
                throw new JsonException(string.Format("Unknown Criteria operator: {0}", opStr));

            if (op != CriteriaOperator.Like && op != CriteriaOperator.NotLike)
                throw new JsonException(string.Format("Invalid Criteria format: {0}", array.ToString()));

            if (array[3].ValueKind != JsonValueKind.String ||
                array[3].GetString() is not string escapeStr ||
                escapeStr.Length != 1)
                throw new JsonException(string.Format("Invalid Criteria escape character: {0}", array.ToString()));

            try
            {
                return new BinaryCriteria(ParseValue(array[0]), op, ParseValue(array[2]), escapeStr[0]);
            }
            catch (ArgumentException ex)
            {
                throw new JsonException(string.Format("Invalid Criteria escape character: {0}", array.ToString()), ex);
            }
        }

        throw new JsonException(string.Format("Can't deserialize {0} as Criteria item", array[0].ToString()));
    }

    private static readonly string[] OperatorToKey =
    [
        "()", // Paren
        "not", // Not
        "is null", // IsNull
        "is not null", // IsNotNull
        "exists", // Exists
        "and", // AND
        "or", // |
        "xor", // XOR
        "=", // EQ
        "!=", // NE
        ">", // GT
        ">=", // GE
        "<", // LT
        "<=", // LE
        "in", // IN
        "not in", // NOT IN
        "like", // LIKE
        "not like" // NOT LIKE
    ];

    private static readonly Dictionary<string, CriteriaOperator> KeyToOperator;
}
