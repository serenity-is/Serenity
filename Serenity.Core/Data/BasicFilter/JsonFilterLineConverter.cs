using System;
using Newtonsoft.Json;

namespace Serenity.Data
{
    /// <summary>
    ///   Serialize/deserialize a row</summary>
    public class JsonFilterLineConverter : JsonConverter
    {
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
            ToJson(writer, (FilterLine)value);
        }

        private void ToJson(JsonWriter writer, FilterLine line)
        {
            writer.WriteStartObject();
            if (line.Field != null)
            {
                writer.WritePropertyName("f");
                writer.WriteValue(line.Field);
            }

            writer.WritePropertyName("o");
            writer.WriteValue(Enum.GetName(typeof(FilterOp), line.Op));

            if (line.OR)
            {
                writer.WritePropertyName("or");
                writer.WriteValue(1);
            }

            if (line.Value != null)
            {
                writer.WritePropertyName("v");
                writer.WriteValue(line.Value);
            }

            if (line.Value2 != null)
            {
                writer.WritePropertyName("v2");
                writer.WriteValue(line.Value2);
            }

            if (line.LeftParen)
            {
                writer.WritePropertyName("l");
                writer.WriteValue(1);
            }

            if (line.RightParen)
            {
                writer.WritePropertyName("r");
                writer.WriteValue(1);
            }

            if (line.Values != null)
            {
                writer.WritePropertyName("vs");
                writer.WriteStartArray();
                foreach (var v in line.Values)
                    writer.WriteValue(v);
                writer.WriteEndArray();
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
            throw new JsonSerializationException("Not Implemented");
        }

        /// <summary>
        ///   Determines whether this instance can convert the specified object type.</summary>
        /// <param name="objectType">
        ///   Type of the object.</param>
        /// <returns>
        ///   True if this instance can convert the specified object type; otherwise, false.</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FilterLine);
        }

        /// <summary>
        ///   Gets a value indicating whether this <see cref="JsonConverter"/> can read JSON.</summary>
        /// <value>
        ///   True if this <see cref="JsonConverter"/> can write JSON; otherwise, false.</value>
        public override bool CanRead
        {
            get { return false; }
        }

        /// <summary>
        ///   Gets a value indicating whether this <see cref="JsonConverter"/> can write JSON.</summary>
        /// <value>
        ///   True if this <see cref="JsonConverter"/> can write JSON; otherwise, false.</value>
        public override bool CanWrite
        {
            get { return true; }
        }
    }
}