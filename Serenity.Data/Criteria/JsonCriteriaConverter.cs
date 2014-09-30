using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections;

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
                serializer.Serialize(writer, ((ParamCriteria)criteria).Name);
                return;
            }

            if (criteria is Criteria)
            {
                serializer.Serialize(writer, ((Criteria)criteria).Expression);
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
                writer.WriteValue(OperatorToKey[(int)c.Operator]);
                ToJson(writer, c.LeftOperand, serializer);
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
            throw new NotImplementedException();

            /*
            if (reader.TokenType == JsonToken.Null)
                return null;

            var row = (Row)(Activator.CreateInstance(objectType));
            if (row == null)
                throw new JsonSerializationException(String.Format("No row of type {0} could be created.", objectType.Name));

            row.TrackAssignments = true;

            if (!reader.Read())
                throw new JsonSerializationException("Unexpected end when deserializing object.");

            int initialDepth = reader.Depth;
            do
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        string fieldName = (string)reader.Value;
                        
                        if (!reader.Read())
                            throw new JsonSerializationException("Unexpected end when deserializing object.");

                        var field = row.FindField(fieldName);
                        if (field == null)
                            field = row.FindFieldByPropertyName(fieldName);
                        
                        if (field == null &&
                            serializer.MissingMemberHandling == MissingMemberHandling.Error)
                            throw new JsonSerializationException(String.Format("Could not find field '{0}' on row of type '{1}'", fieldName, objectType.Name));

                        while (reader.TokenType == JsonToken.Comment)
                            reader.Read();

                        if (field == null)
                            reader.Skip();
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

            throw new JsonSerializationException("Unexpected end when deserializing object.");*/
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
            "!", // Not
            "(null)", // IsNull
            "(!null)", // IsNotNull
            "(exists)", // Exists
            "&", // AND
            "|", // |
            "^", // XOR
            "=", // EQ
            "!=", // NE
            ">", // GT
            ">=", // GE
            "<", // LT
            "<=", // LE
            "(in)", // IN
            "(!in)", // NOT IN
            "~", // LIKE
            "!~" // NOT LIKE
        };

        private static readonly Dictionary<string, CriteriaOperator> KeyToOperator;
    }
}