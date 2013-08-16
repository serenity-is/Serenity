using Serenity.Data;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Web.Hosting;

namespace Serenity.Web
{
    public static class JsConfigHelper
    {
        public static string RemovePrefix(string json)
        {
            json = json.TrimToNull();

            if (json != null && json.StartsWith("var "))
            {
                var eq = json.IndexOf("=");
                if (eq > 0)
                    json = json.Substring(eq + 1).TrimToNull();
            }

            return json;
        }

        public static string JsConfigPrefix = "var config =" + Environment.NewLine;

        public static string AddPrefix(string json)
        {
            json = json.TrimToNull();
            if (json == null)
                throw new ArgumentNullException("json");

            return JsConfigPrefix + json;
        }

        public static TConfig LoadConfig<TConfig>(string path)
            where TConfig : new()
        {
            if (path.StartsWith("~/"))
                path = HostingEnvironment.MapPath(path);

            if (File.Exists(path))
                using (var sr = new StreamReader(path))
                {
                    string json = JsConfigHelper.RemovePrefix(sr.ReadToEnd()) ?? "{}";
                    return JsonConvert.DeserializeObject<TConfig>(json, JsonSettings.Tolerant);
                }

            return new TConfig();
        }
    }
}