namespace Serenity.Reporting;

/// <summary>
/// Attribute used to set the <see cref="ICellDecorator"/> type
/// for a property.
/// </summary>
/// <remarks>
/// Creates an instance of the class.
/// </remarks>
/// <param name="decorator">Decorator type</param>
public class CellDecoratorAttribute(Type decorator) : Attribute
{

    /// <summary>
    /// Gets the decorator type.
    /// </summary>
    public Type DecoratorType { get; set; } = decorator;
}
