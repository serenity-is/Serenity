using System;
using System.Collections.Generic;
using Serenity.Data;
using Serenity.Web.PropertyEditor;
using Serenity.Reflection;

namespace Serenity.Web
{
    public class FormScript : INamedDynamicScript
    {
        public class Data
        {
            public PropertyItem[] Items { get; set; }
        }

        private string _name;
        private string _scriptName;
        private Type _formType;
        private EventHandler _scriptChanged;

        public FormScript(string name, Type formType)
        {
            _formType = formType;
            _name = name;
            _scriptName = "Form." + name;
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
            var items = PropertyEditorHelper.GetPropertyItemsFor(_formType);
            if (typeof(ICustomizedFormScript).IsAssignableFrom(_formType))
            {
                var instance = InstanceCreator.GetInstance(_formType) as ICustomizedFormScript;
                instance.Customize(items);
            }

            return String.Format("Q$ScriptData.set({0}, {1});", ("Form." + _name).ToSingleQuoted(), items.ToJsonString());
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