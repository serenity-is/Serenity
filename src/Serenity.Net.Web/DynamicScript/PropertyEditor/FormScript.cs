using Serenity.PropertyGrid;

namespace Serenity.Web;

/// <summary>
/// Dynamic script type for forms (<see cref="FormScriptAttribute"/>)
/// </summary>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="name">Script name</param>
/// <param name="formType">Columns type</param>
/// <param name="propertyProvider">Property item provider</param>
/// <param name="serviceProvider">Service provider</param>
public class FormScript(string name, Type formType, IPropertyItemProvider propertyProvider,
    IServiceProvider serviceProvider) : PropertyItemsScript("Form." + CheckName(name), formType, propertyProvider, serviceProvider)
{
}