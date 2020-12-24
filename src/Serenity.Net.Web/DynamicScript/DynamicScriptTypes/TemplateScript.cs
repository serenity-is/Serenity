using System;
using System.Globalization;

namespace Serenity.Web
{
    public class TemplateScript : DynamicScript, INamedDynamicScript
    {
        private readonly string key;
        private readonly Func<string> getTemplate;

        public TemplateScript(string key, Func<string> getTemplate)
        {
            this.getTemplate = getTemplate ?? throw new ArgumentNullException(nameof(getTemplate));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public string ScriptName => "Template." + key;

        public override string GetScript()
        {
            string templateText = getTemplate();

            return string.Format(CultureInfo.InvariantCulture, "Q.ScriptData.set({0}, {1})", 
                ("Template." + key).ToSingleQuoted(),
                templateText.ToSingleQuoted()); 
        }
    }
}