using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using Serenity.Data;

namespace Serenity.Web
{
    public class LocalInitScript : INamedDynamicScript
    {
        private string _scriptName;
        private int _languageId;
        private EventHandler _scriptChanged;

        public LocalInitScript(int languageId)
        {
            _languageId = languageId;
            _scriptName = GetScriptName(languageId);
        }

        public bool NonCached { get { return false; } }

        public static string GetScriptName(int languageId)
        {
            return String.Format("LocalInit.{0}", languageId.ToInvariant());
        }

        public void Changed()
        {
            if (_scriptChanged != null)
                _scriptChanged(this, new EventArgs());
        }

        public string ScriptName { get { return _scriptName; } }

        public static string GetLocalInitScript()
        {
            return "";
            // TODO: return TemplateHelper.RenderTemplate<dynamic>("~/Views/Script/LocalInit.cshtml", null);
        }

        public string GetScript()
        {
            return GetLocalInitScript();
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