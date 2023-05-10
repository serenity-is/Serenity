using Newtonsoft.Json.Linq;
using System.Collections;

namespace Serenity.Data;

/// <summary>
///   Serialize/deserialize a BaseCriteria object</summary>
public class JsonCriteriaConverter : JsonConverter
{
    static JsonCriteriaConverter()
    {
        KeyToOperator = new Dictionary<string, CriteriaOperator>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < OperatorToKey.Length; i++)
            KeyToOperator[OperatorToKey[i]] = (CriteriaOperator)i;
    }

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
        ToJson(writer, (BaseCriteria)value, serializer);
    }

    private void ToJson(JsonWriter writer, BaseCriteria criteria, JsonSerializer serializer)
    {
        if (criteria is null ||
            criteria.IsEmpty)
        {
            writer.WriteNull();
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
                serializer.Serialize(writer, value);
                writer.WriteEndArray();
                return;
            }

            serializer.Serialize(writer, value);
            return;
        }

        if (criteria is ParamCriteria prm)
        {
            writer.WriteStartArray();
            serializer.Serialize(writer, prm.Name);
            writer.WriteEndArray();
            return;
        }

        if (criteria is Criteria crit)
        {
            writer.WriteStartArray();
            serializer.Serialize(writer, crit.Expression);
            writer.WriteEndArray();
            return;
        }

        if (criteria is UnaryCriteria unary)
        {
            writer.WriteStartArray();
            writer.WriteValue(OperatorToKey[(int)unary.Operator]);
            ToJson(writer, unary.Operand, serializer);
            writer.WriteEndArray();
            return;
        }

        if (criteria is BinaryCriteria binary)
        {
            writer.WriteStartArray();
            ToJson(writer, binary.LeftOperand, serializer);
            writer.WriteValue(OperatorToKey[(int)binary.Operator]);
            ToJson(writer, binary.RightOperand, serializer);
            writer.WriteEndArray();
            return;
        }

        throw new JsonSerializationException(string.Format("Can't serialize criteria of type {0}", criteria.GetType().FullName));
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

        var value = serializer.Deserialize<JArray>(reader);
        return Parse(value);
    }

    private BaseCriteria ParseValue(JToken value)
    {
        if (value is JValue jvalue)
            return new ValueCriteria(jvalue.Value);

        if (value is JArray array)
            return Parse(array);

        throw new JsonSerializationException(string.Format("Can't deserialize {0} as Criteria value", value.ToString()));
    }

    private BaseCriteria Parse(JArray array)
    {
        if (array == null)
            return Criteria.Empty;

        if (array.Count == 0)
            throw new JsonSerializationException("Can't deserialize empty array as Criteria");

        if (array.Count == 1)
        {
            if (array[0] is JArray jArray)
            {
                var list = new List<object>();
                foreach (var item in jArray)
                {
                    if (item == null)
                        throw new ArgumentNullException("item");

                    if (item is not JValue)
                        throw new JsonSerializationException(string.Format("Can't deserialize {0} as Criteria value", item.ToString()));

                    list.Add(((JValue)item).Value);
                }

                return new ValueCriteria(list.ToArray());
            }

            if (array[0] is not JValue jValue || jValue.Value is not string)
                throw new JsonSerializationException(string.Format("Couldn't deserialize string criteria: {0}", array.ToString()));

            var value = (string)((JValue)array[0]).Value;
            if (value == null)
                throw new JsonSerializationException(string.Format("Null Criteria expression: {0}", array.ToString()));

            if (value.StartsWith("@"))
                return new ParamCriteria(value);

            return new Criteria(value);
        }

        if (array.Count == 2)
        {
            if (array[0] is not JValue jValue || jValue.Value is not string)
                throw new JsonSerializationException(string.Format("Couldn't deserialize unary criteria: {0}", array.ToString()));

            var opStr = (string)((JValue)array[0]).Value;

            if (!KeyToOperator.TryGetValue(opStr, out CriteriaOperator op))
                throw new JsonSerializationException(string.Format("Unknown Criteria operator: {0}", opStr));

            if (op < CriteriaOperator.Paren || op > CriteriaOperator.Exists)
                throw new JsonSerializationException(string.Format("Invalid Unary Criteria format: {0}", array.ToString()));

            return new UnaryCriteria(op, ParseValue(array[1]));
        }

        if (array.Count == 3)
        {
            if (array[1] is not JValue jValue || jValue.Value is not string)
                throw new JsonSerializationException(string.Format("Couldn't deserialize unary criteria: {0}", array.ToString()));

            var opStr = (string)((JValue)array[1]).Value;

            if (!KeyToOperator.TryGetValue(opStr, out CriteriaOperator op))
                throw new JsonSerializationException(string.Format("Unknown Criteria operator: {0}", opStr));

            if (op < CriteriaOperator.AND || op > CriteriaOperator.NotLike)
                throw new JsonSerializationException(string.Format("Invalid Criteria format: {0}", array.ToString()));

            return new BinaryCriteria(ParseValue(array[0]), op, ParseValue(array[2]));
        }

        throw new JsonSerializationException(string.Format("Can't deserialize {0} as Criteria item", array[0].ToString()));
    }

    /// <summary>
    ///   Determines whether this instance can convert the specified object type.</summary>
    /// <param name="objectType">
    ///   Type of the object.</param>
    /// <returns>
    ///   True if this instance can convert the specified object type; otherwise, false.</returns>
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsSubclassOf(typeof(BaseCriteria));
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

    private static readonly string[] OperatorToKey =
    {
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
    };

    private static readonly Dictionary<string, CriteriaOperator> KeyToOperator;
}