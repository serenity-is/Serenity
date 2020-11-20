using Serenity.Abstractions;
using Serenity.ComponentModel;
using Serenity.PropertyGrid;
using Serenity.Web.PropertyEditor;
using System;
using System.Linq;

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
        private IPropertyItemRegistry registry;

        protected PropertyItemsScript(string scriptName, Type type, IPropertyItemRegistry registry)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
            this.scriptName = scriptName;
            this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
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
            var items = registry.GetPropertyItemsFor(type).ToList();
            if (typeof(ICustomizedFormScript).IsAssignableFrom(type))
            {
                var instance = Activator.CreateInstance(type) as ICustomizedFormScript;
                instance.Customize(items);
            }

            return String.Format("Q.ScriptData.set({0}, {1});", (scriptName).ToSingleQuoted(), items.ToJson());
        }

        public void CheckRights(IPermissionService permissions, ITextLocalizer localizer)
        {
        }

        public event System.EventHandler ScriptChanged
        {
            add { scriptChanged += value; }
            remove { scriptChanged -= value; }
        }
    }
}