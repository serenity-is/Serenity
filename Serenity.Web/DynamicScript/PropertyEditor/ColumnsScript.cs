using System;

namespace Serenity.Web
{
    public class ColumnsScript : PropertyItemsScript
    {
        public ColumnsScript(string name, Type columnsType)
            : base("Columns." + CheckName(name), columnsType)
        {
        }
    }
}