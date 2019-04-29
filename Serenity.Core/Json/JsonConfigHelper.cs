using Newtonsoft.Json;
using System.IO;

namespace Serenity.Configuration
{
    /// <summary>
    /// Helper class for loading JSON config from a file.
    /// </summary>
    public static class JsonConfigHelper
    {
        /// <summary>
        /// Loads the JSON configuration from specified file.
        /// </summary>
        /// <typeparam name="TConfig">The type of the configuration.</typeparam>
        /// <param name="path">The path.</param>
        /// <returns></returns>
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