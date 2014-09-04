using System;
using System.Collections.Generic;
using Serenity.Data;

namespace Serenity.Web.FilterPanel
{
    public class FilterFieldsScript : INamedDynamicScript
    {
        private string _name;
        private string _scriptName;
        private Func<IEnumerable<IFilterField>> _getFields;
        private EventHandler _scriptChanged;

        public FilterFieldsScript(string name, Func<IEnumerable<IFilterField>> getFields)
        {
            _getFields = getFields;
            _name = name;
            _scriptName = "FilterFields." + name;
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
            var items = _getFields();
            return String.Format("$.nt.filterFields.{0} = {1};", _name, items.ToJson());
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