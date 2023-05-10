namespace Serenity.Reporting;

/// <summary>
/// Attribute used to set the <see cref="ICellDecorator"/> type
/// for a property.
/// </summary>
public class CellDecoratorAttribute : Attribute
{
    /// <summary>
    /// Creates an instance of the class.
    /// </summary>
    /// <param name="decorator">Decorator type</param>
    public CellDecoratorAttribute(Type decorator)
    {
        DecoratorType = decorator;
    }

    /// <summary>
    /// Gets the decorator type.
    /// </summary>
    public Type DecoratorType { get; set; }
}
