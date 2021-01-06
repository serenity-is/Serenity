using Serenity.PropertyGrid;
using System;

namespace Serenity.Web
{
    public class ColumnsScript : PropertyItemsScript
    {
        public ColumnsScript(string name, Type columnsType, IPropertyItemProvider propertyProvider, 
            IServiceProvider serviceProvider)
            : base("Columns." + CheckName(name), columnsType, 
                 propertyProvider, serviceProvider)
        {
        }
    }
}