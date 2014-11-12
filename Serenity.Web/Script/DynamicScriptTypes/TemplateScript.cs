using System;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.Hosting;

namespace Serenity.Web
{
    public class TemplateScript : INamedDynamicScript
    {
        //private static readonly Regex _whiteSpaceRegex = new Regex("\\s{2,}", RegexOptions.Compiled);

        private string _scriptName;
        private string _template;
        private string _key;
        private EventHandler _scriptChanged;

        public TemplateScript(string key, string template)
        {
            _template = template;
            _key = key;
            _scriptName = "Template." + key;
            DynamicScriptManager.Register(this);
        }

        public TimeSpan Expiration { get; set; }
        public string GroupKey { get; set; }

        public void Changed()
        {
            if (_scriptChanged != null)
                _scriptChanged(this, new EventArgs());
        }

        public string ScriptName { get { return _scriptName; } }

        public string GetScript()
        {
            string templateText;
            
            if (_template.ToLowerInvariant().EndsWith(".html"))
            {
                using (var sr = new StreamReader(HostingEnvironment.MapPath(_template)))
                    templateText = sr.ReadToEnd();
            }
            else
            {
                templateText = TemplateHelper.RenderViewToString(_template, null);
            } 

            return "Q$ScriptData.set('Template." + _key + "', " + templateText.ToSingleQuoted() + ");"; 
 	    }

        public void CheckRights()
        {
        }

        public event System.EventHandler ScriptChanged
        {
            add { _scriptChanged += value; }
            remove { _scriptChanged -= value; }
        }
    }
}