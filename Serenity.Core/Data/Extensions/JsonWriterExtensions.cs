using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using Newtonsoft.Json;

namespace Serenity.Data
{
    /// <summary>
    ///   Class that contains extension methods for JsonWriter</summary>
    public static class JsonWriterExtensions
    {
        /// <summary>
        ///   Writes the value to JSON output with specified name.</summary>
        /// <param name="jw">
        ///   JSON writer.</param>
        /// <param name="name">
        ///   Field name (required).</param>
        /// <param name="value">
        ///   Field value.</param>
        public static void WriteValue(this JsonWriter jw, string name, int? value)
        {
            if (name.IsNullOrEmpty())
                throw new ArgumentNullException("name");

            jw.WritePropertyName(name);
            jw.WriteValue(value);
        }

        /// <summary>
        ///   Writes the value to JSON output with specified name.</summary>
        /// <param name="jw">
        ///   JSON writer.</param>
        /// <param name="name">
        ///   Field name (required).</param>
        /// <param name="value">
        ///   Field value.</param>
        public static void WriteValue(this JsonWriter jw, string name, string value)
        {
            if (name.IsNullOrEmpty())
                throw new ArgumentNullException("name");

            jw.WritePropertyName(name);
            jw.WriteValue(value);
        }

        /// <summary>
        ///   Writes the value to JSON output with specified name.</summary>
        /// <param name="jw">
        ///   JSON writer.</param>
        /// <param name="name">
        ///   Field name (required).</param>
        /// <param name="value">
        ///   Field value.</param>
        public static void WriteValue(this JsonWriter jw, string name, decimal? value)
        {
            if (name.IsNullOrEmpty())
                throw new ArgumentNullException("name");

            jw.WritePropertyName(name);
            jw.WriteValue(value);
        }

        /// <summary>
        ///   Writes the value to JSON output with specified name.</summary>
        /// <param name="jw">
        ///   JSON writer.</param>
        /// <param name="name">
        ///   Field name (required).</param>
        /// <param name="value">
        ///   Field value.</param>
        public static void WriteValue(this JsonWriter jw, string name, Int64? value)
        {
            if (name.IsNullOrEmpty())
                throw new ArgumentNullException("name");

            jw.WritePropertyName(name);
            jw.WriteValue(value);
        }

        /// <summary>
        ///   Writes the value to JSON output with specified name.</summary>
        /// <param name="jw">
        ///   JSON writer.</param>
        /// <param name="name">
        ///   Field name (required).</param>
        /// <param name="value">
        ///   Field value.</param>
        public static void WriteValue(this JsonWriter jw, string name, double? value)
        {
            if (name.IsNullOrEmpty())
                throw new ArgumentNullException("name");

            jw.WritePropertyName(name);
            jw.WriteValue(value);
        }

        /// <summary>
        ///   Writes the value to JSON output with specified name.</summary>
        /// <param name="jw">
        ///   JSON writer.</param>
        /// <param name="name">
        ///   Field name (required).</param>
        /// <param name="value">
        ///   Field value.</param>
        public static void WriteValue(this JsonWriter jw, string name, bool? value)
        {
            if (name.IsNullOrEmpty())
                throw new ArgumentNullException("name");

            jw.WritePropertyName(name);
            jw.WriteValue(value);
        }

        /// <summary>
        ///   Converts an object to its JSON representation</summary>
        /// <param name="value">
        ///   Object</param>
        /// <returns>
        ///   JSON representation string.</returns>
        /// <remarks>
        ///   null, Int32, Boolean, DateTime, Decimal, Double, Guid types handled automatically.
        ///   If object has a ToJson method it is used, otherwise value.ToString() is used as last fallback.</remarks>
        public static string ToJsonString(this object value)
        {
            return Json.Serialize(value);
        }


        /// <summary>
        ///   Converts an object to its JSON representation optionally ignoring circular references</summary>
        /// <param name="value">
        ///   Object</param>
        /// <param name="jw">
        ///   JSON writer.</param>
        /// <param name="ignoreCircular">
        ///   True to ignore circulare references and output an empty ({}) object instead. </param>
        /// <remarks>
        ///   null, Int32, Boolean, DateTime, Decimal, Double, Guid types handled automatically.
        ///   If object has a ToJson method it is used, otherwise value.ToString() is used as last fallback.</remarks>
        public static void ToJson(this object value, JsonWriter jw)
        {
            JsonSerializer.Create(Json.DefaultSettings).Serialize(jw, value);
        }
    }
}
