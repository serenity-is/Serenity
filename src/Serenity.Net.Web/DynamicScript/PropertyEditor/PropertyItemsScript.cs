using Microsoft.Extensions.DependencyInjection;
using Serenity.Abstractions;
using Serenity.ComponentModel;
using Serenity.PropertyGrid;
using Serenity.Web.PropertyEditor;
using System;
using System.Globalization;
using System.Linq;

namespace Serenity.Web
{
    public abstract class PropertyItemsScript : INamedDynamicScript
    {
        public class Data
        {
            public PropertyItem[] Items { get; set; }
        }

        private readonly string scriptName;
        private readonly Type type;
        private readonly IServiceProvider serviceProvider;
        private readonly IPropertyItemProvider propertyProvider;
        private EventHandler scriptChanged;

        protected PropertyItemsScript(string scriptName, Type type, 
            IPropertyItemProvider propertyProvider, IServiceProvider serviceProvider)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
            this.serviceProvider = serviceProvider ?? 
                throw new ArgumentNullException(nameof(serviceProvider));
            this.propertyProvider = propertyProvider ?? 
                throw new ArgumentNullException(nameof(PropertyItemsScript.propertyProvider));
            this.scriptName = scriptName;
        }

        protected static string CheckName(string name)
        {
            if (name.IsEmptyOrNull())
                throw new ArgumentNullException(nameof(name));

            return name;
        }

        public TimeSpan Expiration { get; set; }
        public string GroupKey { get; set; }

        public void Changed()
        {
            scriptChanged?.Invoke(this, new EventArgs());
        }

        public string ScriptName => scriptName;

        public string GetScript()
        {
            var items = propertyProvider.GetPropertyItemsFor(type).ToList();
            if (typeof(ICustomizedFormScript).IsAssignableFrom(type))
            {
                var instance = ActivatorUtilities.CreateInstance(
                    serviceProvider, type) as ICustomizedFormScript;
                instance.Customize(items);
            }

            return string.Format(CultureInfo.InvariantCulture, "Q.ScriptData.set({0}, {1});", 
                scriptName.ToSingleQuoted(), items.ToJson());
        }

        public void CheckRights(IPermissionService permissions, ITextLocalizer localizer)
        {
        }

        public event EventHandler ScriptChanged
        {
            add { scriptChanged += value; }
            remove { scriptChanged -= value; }
        }
    }
}