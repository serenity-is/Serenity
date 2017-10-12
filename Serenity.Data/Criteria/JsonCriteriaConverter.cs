using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace Serenity.Data
{
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
            if (Object.ReferenceEquals(null, criteria) ||
                criteria.IsEmpty)
            {
                writer.WriteNull();
                return;
            }

            if (criteria is ValueCriteria)
            {
                var value = ((ValueCriteria)criteria).Value;
                if (value != null && value is IEnumerable && !(value is string))
                {
                    // make sure that first value in array won't be recognized as a operator
                    // while deserializing (e.g. if values are [">", "a", "b"], serialize them
                    // as [[">", "a", "b"]] so that the first > won't be recognized as GE operator.
                    writer.WriteStartArray();
                    serializer.Serialize(writer, value);
                    writer.WriteEndArray();
                    return;
                }

                serializer.Serialize(writer, value );
                return;
            }

            if (criteria is ParamCriteria)
            {
                writer.WriteStartArray();
                serializer.Serialize(writer, ((ParamCriteria)criteria).Name);
                writer.WriteEndArray();
                return;
            }

            if (criteria is Criteria)
            {
                writer.WriteStartArray();
                serializer.Serialize(writer, ((Criteria)criteria).Expression);
                writer.WriteEndArray();
                return;
            }

            if (criteria is UnaryCriteria)
            {
                writer.WriteStartArray();
                var c = (UnaryCriteria)criteria;
                writer.WriteValue(OperatorToKey[(int)c.Operator]);
                ToJson(writer, c.Operand, serializer);
                writer.WriteEndArray();
                return;
            }

            if (criteria is BinaryCriteria)
            {
                writer.WriteStartArray();
                var c = (BinaryCriteria)criteria;
                ToJson(writer, c.LeftOperand, serializer);
                writer.WriteValue(OperatorToKey[(int)c.Operator]);
                ToJson(writer, c.RightOperand, serializer);
                writer.WriteEndArray();
                return;
            }

            throw new JsonSerializationException(String.Format("Can't serialize criteria of type {0}", criteria.GetType().FullName));
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
            if (value is JValue)
            {
                return new ValueCriteria(((JValue)value).Value);
            }

            if (value is JArray)
            {
                return Parse((JArray)value);
            }

            throw new JsonSerializationException(String.Format("Can't deserialize {0} as Criteria value", value.ToString()));
        }

        private BaseCriteria Parse(JArray array)
        {
            if (array == null)
                return Criteria.Empty;

            if (array.Count == 0)
                throw new JsonSerializationException("Can't deserialize empty array as Criteria");

            if (array.Count == 1)
            {
                if (array[0] is JArray)
                {
                    var list = new List<object>();
                    foreach (var item in (JArray)array[0])
                    {
                        if (item == null)
                            throw new ArgumentNullException("item");

                        if (!(item is JValue))
                            throw new JsonSerializationException(String.Format("Can't deserialize {0} as Criteria value", item.ToString()));

                        list.Add(((JValue)item).Value);
                    }

                    return new ValueCriteria(list.ToArray());
                }

                if (!(array[0] is JValue) ||
                    !(((JValue)array[0]).Value is string))
                {
                    throw new JsonSerializationException(String.Format("Couldn't deserialize string criteria: {0}", array.ToString()));
                }

                var value = (string)((JValue)array[0]).Value;
                if (value == null)
                    throw new JsonSerializationException(String.Format("Null Criteria expression: {0}", array.ToString()));

                if (value.StartsWith("@"))
                    return new ParamCriteria(value);

                return new Criteria(value);
            }

            if (array.Count == 2)
            {
                if (!(array[0] is JValue) ||
                    !(((JValue)array[0]).Value is string))
                {
                    throw new JsonSerializationException(String.Format("Couldn't deserialize unary criteria: {0}", array.ToString()));
                }

                var opStr = (string)((JValue)array[0]).Value;

                CriteriaOperator op;
                if (!KeyToOperator.TryGetValue(opStr, out op))
                    throw new JsonSerializationException(String.Format("Unknown Criteria operator: {0}", opStr));

                if (op < CriteriaOperator.Paren || op > CriteriaOperator.Exists)
                {
                    throw new JsonSerializationException(String.Format("Invalid Unary Criteria format: {0}", array.ToString()));
                }

                return new UnaryCriteria(op, ParseValue(array[1]));
            }

            if (array.Count == 3)
            {
                if (!(array[1] is JValue) ||
                    !(((JValue)array[1]).Value is string))
                {
                    throw new JsonSerializationException(String.Format("Couldn't deserialize unary criteria: {0}", array.ToString()));
                }

                var opStr = (string)((JValue)array[1]).Value;

                CriteriaOperator op;
                if (!KeyToOperator.TryGetValue(opStr, out op))
                    throw new JsonSerializationException(String.Format("Unknown Criteria operator: {0}", opStr));

                if (op < CriteriaOperator.AND || op > CriteriaOperator.FullTextSearchStartWith)
                    throw new JsonSerializationException(String.Format("Invalid Criteria format: {0}", array.ToString()));

                return new BinaryCriteria(ParseValue(array[0]), op, ParseValue(array[2]));
            }

            throw new JsonSerializationException(String.Format("Can't deserialize {0} as Criteria item", array[0].ToString()));
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
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        ///   Gets a value indicating whether this <see cref="JsonConverter"/> can write JSON.</summary>
        /// <value>
        ///   True if this <see cref="JsonConverter"/> can write JSON; otherwise, false.</value>
        public override bool CanWrite
        {
            get { return true; }
        }

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
            "not like", // NOT LIKE
            "contains", // FULL TEXT SEARCH - CONTAINS
            "startswith" // FULL TEXT SEARCH - STARTS WITH
        };

        private static readonly Dictionary<string, CriteriaOperator> KeyToOperator;
    }
}