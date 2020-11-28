using Serenity.PropertyGrid;
using System;

namespace Serenity.Web
{
    public class ColumnsScript : PropertyItemsScript
    {
        public ColumnsScript(string name, Type columnsType, IPropertyItemProvider registry)
            : base("Columns." + CheckName(name), columnsType, registry)
        {
        }
    }
}