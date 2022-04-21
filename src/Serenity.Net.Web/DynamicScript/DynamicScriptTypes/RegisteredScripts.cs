using System.IO;

namespace Serenity.Web
{
    public class RegisteredScripts : DynamicScript, INamedDynamicScript
    {
        private readonly IDynamicScriptManager scriptManager;

        public string ScriptName => "RegisteredScripts";

        public RegisteredScripts(IDynamicScriptManager scriptManager)
        {
            Expiration = TimeSpan.FromDays(-1);
            this.scriptManager = scriptManager;
        }

        public override string GetScript()
        {
            return "Q.ScriptData.setRegisteredScripts(" +
                ToJsonFast(scriptManager.GetRegisteredScripts()) + ");";
        }

        private static string ToJsonFast(IDictionary<string, string> dictionary)
        {
            var sw = new StringWriter();
            var writer = new JsonTextWriter(sw);
            writer.WriteStartObject();
            foreach (var pair in dictionary)
            {
                writer.WritePropertyName(pair.Key);
                if (pair.Value == null)
                    writer.WriteValue(0);
                else
                    writer.WriteValue(pair.Value);
            }
            writer.WriteEndObject();
            return sw.ToString();
        }
    }
}