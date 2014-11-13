using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Serenity.Web
{
    public class RegisteredScripts : DynamicScript, INamedDynamicScript
    {
        public string ScriptName { get { return "RegisteredScripts"; } }

        public override string GetScript()
        {
            return "Q$ScriptData.setRegisteredScripts(" +
                ToJsonFast(DynamicScriptManager.GetRegisteredScripts()) + ");";
        }

        private static string ToJsonFast(IDictionary<string, string> dictionary)
        {
            var sw = new StringWriter();
            var writer = new JsonTextWriter(sw);
            writer.WriteStartObject();
            foreach (var pair in dictionary)
            {
                writer.WritePropertyName(pair.Key);
                writer.WriteValue(pair.Value);
            }
            writer.WriteEndObject();
            return sw.ToString();
        }
    }
}