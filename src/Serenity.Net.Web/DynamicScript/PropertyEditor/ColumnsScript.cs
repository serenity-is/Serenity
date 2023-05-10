using Serenity.PropertyGrid;

namespace Serenity.Web;

/// <summary>
/// Dynamic script type for columns (<see cref="ColumnsScriptAttribute"/>)
/// </summary>
public class ColumnsScript : PropertyItemsScript
{
    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="name">Script name</param>
    /// <param name="columnsType">Columns type</param>
    /// <param name="propertyProvider">Property item provider</param>
    /// <param name="serviceProvider">Service provider</param>
    public ColumnsScript(string name, Type columnsType, IPropertyItemProvider propertyProvider, 
        IServiceProvider serviceProvider)
        : base("Columns." + CheckName(name), columnsType, 
             propertyProvider, serviceProvider)
    {
    }
}