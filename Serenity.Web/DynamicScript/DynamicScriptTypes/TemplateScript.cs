using System;
using System.IO;

namespace Serenity.Web
{
    public class TemplateScript : DynamicScript, INamedDynamicScript
    {
        private string key;
        private Func<string> getTemplate;

        public TemplateScript(string key, Func<string> getTemplate)
        {
            Check.NotNull(key, "key");
            Check.NotNull(getTemplate, "getTemplate");

            this.getTemplate = getTemplate;
            this.key = key;
        }

        public string ScriptName { get { return "Template." + key; } }

        public override string GetScript()
        {
            string templateText = getTemplate();

            return String.Format("Q$ScriptData.set({0}, {1})", 
                ("Template." + key).ToSingleQuoted(),
                templateText.ToSingleQuoted()); 
        }
    }
}