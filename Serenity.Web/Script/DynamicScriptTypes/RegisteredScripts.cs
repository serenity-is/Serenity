using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using Serenity.Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Serenity.Web
{
    public class RegisteredScripts : INamedDynamicScript
    {
        public const string _scriptName = "RegisteredScripts";
        private EventHandler _scriptChanged;

        public RegisteredScripts()
        {
        }

        public TimeSpan Expiration { get; set; }
        public string GroupKey { get; set; }

        public static string GetScriptName(int languageId)
        {
            return _scriptName;
        }

        public void Changed()
        {
            if (_scriptChanged != null)
                _scriptChanged(this, new EventArgs());
        }

        public string ScriptName { get { return _scriptName; } }

        public string GetScript()
        {
            return "Q$ScriptData.setRegisteredScripts(" + DynamicScriptManager.GetRegisteredScripts().ToJsonFast() + ");";
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

    public static class JsonOptimizations
    {
        public static string ToJsonFast(this IDictionary<string, string> dictionary)
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