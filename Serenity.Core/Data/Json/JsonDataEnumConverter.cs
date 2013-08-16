using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Serenity.Data
{

    /// <summary>
    ///   Serialize/deserialize a data enum</summary>
    public class JsonDataEnumConverter : JsonConverter
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
            if (value == null)
                writer.WriteNull();

            writer.WriteValue(((DataEnum)value).Key);
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
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                case JsonToken.Undefined:
                    return null;
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.Boolean:
                    return DataEnum.ConvertFromInt32(objectType, Convert.ToInt32(reader.Value, CultureInfo.InvariantCulture));
                case JsonToken.String:
                    return DataEnum.ConvertFromString(objectType, (string)reader.Value);
                default:
                    throw new JsonSerializationException("Unexpected token when deserializing row: " + reader.TokenType);
            }
        }

        /// <summary>
        ///   Determines whether this instance can convert the specified object type.</summary>
        /// <param name="objectType">
        ///   Type of the object.</param>
        /// <returns>
        ///   True if this instance can convert the specified object type; otherwise, false.</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsSubclassOf(typeof(DataEnum));
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
    }
}