using Serenity.PropertyGrid;

namespace Serenity.Web;

/// <summary>
/// Dynamic script type for forms (<see cref="FormScriptAttribute"/>).
/// </summary>
/// <remarks>
/// Creates a new instance of the class.
/// </remarks>
/// <param name="name">The script name.</param>
/// <param name="formType">The form type.</param>
/// <param name="propertyProvider">The property item provider.</param>
/// <param name="serviceProvider">The service provider.</param>
public class FormScript(string name, Type formType, IPropertyItemProvider propertyProvider,
    IServiceProvider serviceProvider) : PropertyItemsScript("Form." + CheckName(name), formType, propertyProvider, serviceProvider)
{
}