using Serenity.PropertyGrid;
using System;

namespace Serenity.Web
{
    public class FormScript : PropertyItemsScript
    {
        public FormScript(string name, Type formType, 
            IServiceProvider serviceProvider, IPropertyItemProvider propertyProvider)
            : base("Form." + CheckName(name), formType, 
                  serviceProvider, propertyProvider)
        {
        }
    }
}