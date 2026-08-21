using Serenity.PropertyGrid;

namespace Serenity.Web;

/// <summary>
/// Dynamic script type for columns (<see cref="ColumnsScriptAttribute"/>).
/// </summary>
/// <remarks>
/// Creates a new instance of the class.
/// </remarks>
/// <param name="name">The script name.</param>
/// <param name="columnsType">The columns type.</param>
/// <param name="propertyProvider">The property item provider.</param>
/// <param name="serviceProvider">The service provider.</param>
public class ColumnsScript(string name, Type columnsType, IPropertyItemProvider propertyProvider,
    IServiceProvider serviceProvider) : PropertyItemsScript("Columns." + CheckName(name), columnsType, 
         propertyProvider, serviceProvider)
{
}