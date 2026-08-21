namespace Serenity.Reflection;

/// <summary>
/// An interface to query the list of annotation types for a given type.
/// </summary>
public interface IAnnotationTypeRegistry
{
    /// <summary>
    /// Gets the annotation types for the given type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The list of annotation types that apply to the given type.</returns>
    IEnumerable<Type> GetAnnotationTypesFor(Type type);
}