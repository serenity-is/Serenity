using Serenity.PropertyGrid;

namespace Serenity.Web;

/// <summary>
/// Dynamic script type for columns (<see cref="ColumnsScriptAttribute"/>)
/// </summary>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="name">Script name</param>
/// <param name="columnsType">Columns type</param>
/// <param name="propertyProvider">Property item provider</param>
/// <param name="serviceProvider">Service provider</param>
public class ColumnsScript(string name, Type columnsType, IPropertyItemProvider propertyProvider,
    IServiceProvider serviceProvider) : PropertyItemsScript("Columns." + CheckName(name), columnsType, 
         propertyProvider, serviceProvider)
{
}