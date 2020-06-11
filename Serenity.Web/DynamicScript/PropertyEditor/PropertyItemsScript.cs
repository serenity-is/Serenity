using Serenity.ComponentModel;
using Serenity.Web.PropertyEditor;
using System;
#if !NET45
using System.Reflection;
#endif

namespace Serenity.Web
{
    public abstract class PropertyItemsScript : INamedDynamicScript
    {
        public class Data
        {
            public PropertyItem[] Items { get; set; }
        }

        private string scriptName;
        private Type type;
        private EventHandler scriptChanged;

        protected PropertyItemsScript(string scriptName, Type type)
        {
            this.type = type;
            this.scriptName = scriptName;
        }

        protected static string CheckName(string name)
        {
            if (name.IsEmptyOrNull())
                throw new ArgumentNullException("name");

            return name;
        }

        public TimeSpan Expiration { get; set; }
        public string GroupKey { get; set; }

        public void Changed()
        {
            if (scriptChanged != null)
                scriptChanged(this, new EventArgs());
        }

        public string ScriptName { get { return scriptName; } }

        public string GetScript()
        {
            var items = Serenity.PropertyGrid.PropertyItemHelper.GetPropertyItemsFor(type);
            if (typeof(ICustomizedFormScript).IsAssignableFrom(type))
            {
                var instance = Activator.CreateInstance(type) as ICustomizedFormScript;
                instance.Customize(items);
            }

            return String.Format("Q.ScriptData.set({0}, {1});", (scriptName).ToSingleQuoted(), items.ToJson());
        }

        public void CheckRights()
        {
        }

        public event System.EventHandler ScriptChanged
        {
            add { scriptChanged += value; }
            remove { scriptChanged -= value; }
        }
    }
}