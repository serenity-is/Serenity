using System.Collections;
using System.Text.Json;

namespace Serenity.JsonConverters;

/// <summary>
///   Serialize/deserialize a BaseCriteria object</summary>
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
        if (criteria is null ||
            criteria.IsEmpty)
        {
            writer.WriteNullValue();
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
            writer.WriteEndArray();
            return;
        }

        throw new JsonException(string.Format("Can't serialize criteria of type {0}", criteria.GetType().FullName));
    }

    /// <inheritdoc/>
    public override BaseCriteria Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var value = JsonSerializer.Deserialize<JsonElement[]>(ref reader, options);
        return Parse(value);
    }

    private BaseCriteria ParseValue(JsonElement value)
    {
        if (value.ValueKind == JsonValueKind.String)
            return new ValueCriteria(value.GetString());
        else if (value.ValueKind == JsonValueKind.Number)
            return new ValueCriteria(value.GetDouble());
        else if (value.ValueKind == JsonValueKind.True)
            return new ValueCriteria(true);
        else if (value.ValueKind == JsonValueKind.False)
            return new ValueCriteria(false);

        if (value.ValueKind == JsonValueKind.Array)
            return Parse(JsonSerializer.Deserialize<JsonElement[]>(value));

        throw new JsonException(string.Format("Can't deserialize {0} as Criteria value", value.ToString()));
    }

    private BaseCriteria Parse(JsonElement[] array)
    {
        if (array == null)
            return Criteria.Empty;

        if (array.Length == 0)
            throw new JsonException("Can't deserialize empty array as Criteria");

        if (array.Length == 1)
        {
            if (array[0] is { ValueKind: JsonValueKind.Array } jArray)
            {
                var list = new List<object>();
                foreach (var item in jArray.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Null)
                        throw new ArgumentNullException("item");

                    if (item.ValueKind == JsonValueKind.String)
                        list.Add(item.GetString());
                    else if (item.ValueKind == JsonValueKind.Number)
                        list.Add(item.GetDouble());
                    else if (item.ValueKind == JsonValueKind.True)
                        list.Add(true);
                    else if (item.ValueKind == JsonValueKind.False)
                        list.Add(false);
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

            if (!KeyToOperator.TryGetValue(opStr, out CriteriaOperator op))
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

            if (!KeyToOperator.TryGetValue(opStr, out CriteriaOperator op))
                throw new JsonException(string.Format("Unknown Criteria operator: {0}", opStr));

            if (op < CriteriaOperator.AND || op > CriteriaOperator.NotLike)
                throw new JsonException(string.Format("Invalid Criteria format: {0}", array.ToString()));

            return new BinaryCriteria(ParseValue(array[0]), op, ParseValue(array[2]));
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