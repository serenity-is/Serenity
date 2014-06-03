using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Serenity.Data
{
    public static class Json
    {
        static Json()
        {
        }

        public static JsonReader CreateReader(string input)
        {
            var tw = new StringReader(input);
            return new JsonTextReader(tw);
        }

        public static JsonWriter CreateWriter(TextWriter output)
        {
            return new JsonTextWriter(output);
        }

        public static JsonWriter CreateWriter(StringBuilder sb)
        {
            var tw = new StringWriter(sb);
            return new JsonTextWriter(tw);
        }

        public static T Deserialize<T>(string input)
        {
            return JsonConvert.DeserializeObject<T>(input, JsonSettings.Tolerant);
        }

        public static object Deserialize(string input, Type type)
        {
            return JsonConvert.DeserializeObject(input, type);
        }

        public static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.None, JsonSettings.Tolerant);
        }

        public static string SerializeIndented(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented, JsonSettings.Tolerant);
        }
    }
}
