using Serenity.PropertyGrid;
using System;

namespace Serenity.Web
{
    public class ColumnsScript : PropertyItemsScript
    {
        public ColumnsScript(string name, Type columnsType, 
            IServiceProvider serviceProvider, IPropertyItemProvider propertyProvider)
            : base("Columns." + CheckName(name), columnsType, 
                serviceProvider, propertyProvider)
        {
        }
    }
}