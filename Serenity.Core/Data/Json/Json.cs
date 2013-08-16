using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Serenity.Data
{
    public static class Json
    {
        public static JsonSerializerSettings DefaultSettings;

        static Json()
        {
            DefaultSettings = new JsonSerializerSettings();
            DefaultSettings.NullValueHandling = NullValueHandling.Ignore;
            DefaultSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
            DefaultSettings.TypeNameHandling = TypeNameHandling.None;
            DefaultSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            DefaultSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            DefaultSettings.Converters.Add(new IsoDateTimeConverter());
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
            return JsonConvert.DeserializeObject<T>(input, Json.DefaultSettings);
        }

        public static object Deserialize(string input, Type type)
        {
            return JsonConvert.DeserializeObject(input, type);
        }

        public static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.None, Json.DefaultSettings);
        }

        public static string SerializeIndented(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented, Json.DefaultSettings);
        }
    }
}
