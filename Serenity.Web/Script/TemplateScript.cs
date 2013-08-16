using System;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;

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

        public bool NonCached { get { return false; } }

        public void Changed()
        {
            if (_scriptChanged != null)
                _scriptChanged(this, new EventArgs());
        }

        public string ScriptName { get { return _scriptName; } }

        public string GetScript()
        {
            string templateText = TemplateHelper.RenderViewToString(_template, null);
            return "Q$ScriptData.set('Template." + _key + "', " + templateText.ToSingleQuoted() + ");\n"; 
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