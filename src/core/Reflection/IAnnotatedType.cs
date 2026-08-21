namespace Serenity.Reflection;

/// <summary>
/// Annotated type information.
/// </summary>
public interface IAnnotatedType
{
    /// <summary>
    /// Gets the annotated property.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>The annotated property information.</returns>
    IPropertyInfo GetAnnotatedProperty(PropertyInfo property);
}