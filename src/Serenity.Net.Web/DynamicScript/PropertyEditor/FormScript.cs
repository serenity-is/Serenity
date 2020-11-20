using Serenity.PropertyGrid;
using System;

namespace Serenity.Web
{
    public class FormScript : PropertyItemsScript
    {
        public FormScript(string name, Type formType, IPropertyItemRegistry registry)
            : base("Form." + CheckName(name), formType, registry)
        {
        }
    }
}