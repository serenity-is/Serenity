using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serenity.Data;

namespace Serenity
{
    public static class JsonSettings
    {
        public static JsonSerializerSettings Tolerant;
        public static JsonSerializerSettings Strict;

        static JsonSettings()
        {
            Tolerant = new JsonSerializerSettings();
            Tolerant.DateParseHandling = DateParseHandling.DateTimeOffset;
            Tolerant.NullValueHandling = NullValueHandling.Ignore;
            Tolerant.MissingMemberHandling = MissingMemberHandling.Ignore;
            Tolerant.TypeNameHandling = TypeNameHandling.None;
            Tolerant.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            Tolerant.PreserveReferencesHandling = PreserveReferencesHandling.None;
            Tolerant.Converters.Add(new IsoDateTimeConverter());
            Tolerant.Converters.Add(new JsonSafeInt64Converter());

            Strict = new JsonSerializerSettings();
            Strict.DateParseHandling = DateParseHandling.DateTimeOffset;
            Strict.NullValueHandling = NullValueHandling.Ignore;
            Strict.MissingMemberHandling = MissingMemberHandling.Error;
            Strict.TypeNameHandling = TypeNameHandling.None;
            Strict.ReferenceLoopHandling = ReferenceLoopHandling.Error;
            Strict.PreserveReferencesHandling = PreserveReferencesHandling.None;
            Strict.Converters.Add(new IsoDateTimeConverter());
            Strict.Converters.Add(new JsonSafeInt64Converter());
        }
    }
}
