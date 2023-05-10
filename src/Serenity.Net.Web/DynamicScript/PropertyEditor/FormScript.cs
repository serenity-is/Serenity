using Serenity.PropertyGrid;

namespace Serenity.Web;

/// <summary>
/// Dynamic script type for forms (<see cref="FormScriptAttribute"/>)
/// </summary>
public class FormScript : PropertyItemsScript
{
    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="name">Script name</param>
    /// <param name="formType">Columns type</param>
    /// <param name="propertyProvider">Property item provider</param>
    /// <param name="serviceProvider">Service provider</param>
    public FormScript(string name, Type formType, IPropertyItemProvider propertyProvider, 
        IServiceProvider serviceProvider)
        : base("Form." + CheckName(name), formType, propertyProvider, serviceProvider)
    {
    }
}