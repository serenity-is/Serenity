using Newtonsoft.Json;
using System.IO;

namespace Serenity.Configuration
{
    public static class JsonConfigHelper
    {
        public static TConfig LoadConfig<TConfig>(string path)
            where TConfig : new()
        {
            if (File.Exists(path))
            {
                using (var sr = new StreamReader(File.OpenRead(path)))
                {
                    string json = sr.ReadToEnd().TrimToNull() ?? "{}";
                    return JsonConvert.DeserializeObject<TConfig>(json, JsonSettings.Tolerant);
                }
            }

            return new TConfig();
        }
    }
}