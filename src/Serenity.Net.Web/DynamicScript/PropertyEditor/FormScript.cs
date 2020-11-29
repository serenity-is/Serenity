using Serenity.PropertyGrid;
using System;

namespace Serenity.Web
{
    public class FormScript : PropertyItemsScript
    {
        public FormScript(string name, Type formType, IPropertyItemProvider propertyProvider, 
            IServiceProvider serviceProvider)
            : base("Form." + CheckName(name), formType, propertyProvider, serviceProvider)
        {
        }
    }
}